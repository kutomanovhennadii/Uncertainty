# Policies (numeric behavior) ⚖️

This document summarizes the implementation and defaults for numeric policies used by `Uncertainty.Core` and shows examples for customizing them.

## Overview

Policies control how `UDouble` arithmetic operations handle edge cases and numeric stability. The `UncertaintyPolicies` facade provides a unified, thread-safe interface to configure two categories of policies:

1. **Division Policies** — behavior when dividing by small denominators
2. **Variance Saturation Policies** — bounding finite variance to prevent numerical explosion

---

## Division Policies

### Defaults

- **DivisionTolerance**: `0.0` — denominators are treated as exactly zero only when `mean == 0.0`.
- **DivisionBehavior**: `ThrowOnSmallDenominator` — when a denominator is considered "small" (i.e., `|mean| <= DivisionTolerance`), a `DivideByZeroException` is thrown.
- **DivisionStrategy**: `ThrowingDivisionStrategy` (internal implementation).

### Configuration

#### Setting the Division Tolerance

Use the setter method to change when a denominator is considered "small":

```csharp
// read current value
double tol = UncertaintyPolicies.DivisionTolerance;

// set a new finite non-negative tolerance
UncertaintyPolicies.SetDivisionTolerance(1e-307);
```

#### Changing Division Behavior

Change how small denominators are handled by selecting a built-in behavior:

```csharp
// Throw when |denominator.Mean| <= DivisionTolerance (default)
UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;

// Saturate variance and return a finite result
UncertaintyPolicies.DivisionBehavior = DivisionBehavior.SaturateVariance;

// Attempt to return +/-Infinity as the mean (currently respects contract for finite means)
UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ReturnInfinityMean;
```

Invalid enum values throw `ArgumentOutOfRangeException`.

### Built-in Strategies (Examples)

#### 1. ThrowOnSmallDenominator (Default)

```csharp
UncertaintyPolicies.SetDivisionTolerance(1e-307);
UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;

try
{
    var result = UDouble.FromMeanVar(10, 1) / UDouble.FromMeanVar(1e-308, 0.5);
}
catch (DivideByZeroException)
{
    // thrown when |denominator.Mean| <= DivisionTolerance
}
```

#### 2. SaturateVariance

```csharp
UncertaintyPolicies.SetDivisionTolerance(1e-307);
UncertaintyPolicies.DivisionBehavior = DivisionBehavior.SaturateVariance;

var result = UDouble.FromMeanVar(10, 1) / UDouble.FromMeanVar(1e-308, 0.5);
// result.Mean = 10 / 1e-308 (finite)
// result.Variance is saturated by the variance saturation policy
```

#### 3. ReturnInfinityMean

```csharp
UncertaintyPolicies.SetDivisionTolerance(1e-307);
UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ReturnInfinityMean;

var result = UDouble.FromMeanVar(10, 1) / UDouble.FromMeanVar(1e-308, 0.5);
// Note: Current implementation respects the contract that all means must be finite.
// This may be relaxed in future releases.
```

---

## Variance Saturation Policies

### Purpose

The variance saturation policy is a **numeric-stability safeguard**, not a statistical model. It prevents the computed variance from growing unboundedly when linear error propagation becomes unreliable (e.g., near singularities like division by small values).

### Design

Variance is capped based on a **relative ceiling** derived from the mean:

```
ceiling = max(mean^2 × MaxRelativeVarianceFactor, AbsoluteVarianceMax)
```

where:
- `MaxRelativeVarianceFactor = (MaxRelativeStdDev)^2`
- `MaxRelativeStdDev` is an engineering threshold for "still in the small-error regime"
- `AbsoluteVarianceMax` is the minimum absolute ceiling mandated by the contract

If the computed variance is non-finite or exceeds this ceiling, it is clamped to the ceiling value. Otherwise, it passes through unchanged.

### Configuration

#### Reading Current Options

```csharp
// Get current variance saturation options
VarianceSaturationOptions opts = UncertaintyPolicies.VarianceSaturation;

// Access the parameters
double maxRelStdDev = opts.MaxRelativeStdDev;      // default: 1e8
double absVarMax = opts.AbsoluteVarianceMax;       // default: 1e300
```

#### Configuring Options

Use `ConfigureVarianceSaturation` to update the policy parameters:

```csharp
// Create new options with custom parameters
var newOpts = new VarianceSaturationOptions(
    MaxRelativeStdDev: 1e9,           // increase relative ceiling
    AbsoluteVarianceMax: 1e301        // increase absolute ceiling
);

// Apply configuration (validates and applies thread-safely)
UncertaintyPolicies.ConfigureVarianceSaturation(newOpts);
```

#### Validation

Options are validated when applied. Constraints:

- `MaxRelativeStdDev` must be finite and ≥ 1.0
- `AbsoluteVarianceMax` must be finite and ≥ 1e300 (contract minimum)

```csharp
try
{
    var badOpts = new VarianceSaturationOptions(0.5, 1e300);  // MaxRelativeStdDev < 1.0
    UncertaintyPolicies.ConfigureVarianceSaturation(badOpts);
}
catch (ArgumentOutOfRangeException ex)
{
    // validation error: MaxRelativeStdDev must be >= 1.0
}
```

### Default Behavior

The default saturation policy uses:

```csharp
MaxRelativeStdDev = 1e8
AbsoluteVarianceMax = 1e300
```

This matches the constants in the legacy `VarianceSaturationPolicy` class and ensures all computed variances remain finite and bounded.

### Example

```csharp
var a = UDouble.FromMeanVar(1e-200, 1e-400);
var b = UDouble.FromMeanVar(1e-200, 1e-400);

// Division by very small mean triggers saturation
var result = a / b;

// result.Variance is clamped to the saturation ceiling, not infinity
Console.WriteLine($"Mean: {result.Mean}, Variance: {result.Variance}");
// Output: Mean: 1, Variance: 1E+300 (or less, depending on ceiling calculation)
```

---

## Thread Safety

Both division and variance saturation policies are thread-safe and can be called from async contexts. Configuration methods are synchronous and use `SemaphoreSlim` for short blocking sections. Configuration is typically done once during initialization; repeated changes are usually not expected:

- **Atomic updates**: concurrent calls to configuration methods don't race
- **No deadlocks**: safe to call from both sync and async contexts
- **Predictable blocking**: uses `SemaphoreSlim.Wait()`; configuration is expected to be rare and short-lived

### Thread Safety in Synchronous Code

```csharp
// Safe to call from multiple threads
Task.Run(() => UncertaintyPolicies.SetDivisionTolerance(1e-100));
Task.Run(() => UncertaintyPolicies.ConfigureVarianceSaturation(
    new VarianceSaturationOptions(1e9, 1e301)));
```

### Thread Safety in Asynchronous Code

Safe to call from within `async` methods; the call blocks briefly and is intended to be rare:

```csharp
public async Task InitializePolicies()
{
    // Safe to use in async context
    UncertaintyPolicies.SetDivisionTolerance(1e-100);
    UncertaintyPolicies.DivisionBehavior = DivisionBehavior.SaturateVariance;
    
    // Other async code
    await Task.Delay(100);
    var result = UncertaintyPolicies.DivisionBehavior; // Fast read, no sync
    return result;
}
```

**Why SemaphoreSlim?**
- `SemaphoreSlim` provides safer synchronization for mixed sync/async usage
- The blocking section is intentionally short; configuration is expected to be infrequent
- No deadlock risk even with thread transitions in async pipeline
- Reads (getters) are always fast and don't require synchronization

---

## Summary

| Policy             | Default                          | Configuration                         | Notes                                                |
|--------------------|----------------------------------|---------------------------------------|------------------------------------------------------|
| Division Tolerance | 0.0                              | `SetDivisionTolerance(double)`        | Treated as exactly zero when `\|mean\| <= tolerance` |
| Division Behavior  | `ThrowOnSmallDenominator`        | `DivisionBehavior` property           | Controls action when denominator is small            |
| Variance Saturation| MaxRelStdDev=1e8, AbsVarMax=1e300| `ConfigureVarianceSaturation(options)`| Clamps variance to finite ceiling                    |

For more details on the mathematical model and thread-safety guarantees, see [core-contract.md](Contracts/core-contract.md).