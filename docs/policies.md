# Policies (numeric behavior) ⚖️

This document summarizes the implementation and defaults for numeric policies used by `Uncertainty.Core` and shows examples for customizing them.

## Division behavior

- **Default DivisionTolerance**: `0.0` — denominators are treated as exactly zero only when `mean == 0.0`.
- **Default DivisionBehavior**: `ThrowOnSmallDenominator` — when a denominator is considered "small" (i.e. `|mean| <= DivisionTolerance`), an exception (`DivideByZeroException`) is thrown by default.
- **Default DivisionStrategy**: `ThrowingDivisionStrategy` (internal implementation used by the default `DivisionBehavior`).

If you need a different behaviour you can set `UncertaintyPolicies.DivisionBehavior` to one of the provided enum values. The built-in strategies are internal implementation details; clients should not assign custom strategies.

### Built-in strategies (examples)

1) ThrowOnSmallDenominator (default)

```csharp
Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior =
    Uncertainty.Core.Policies.DivisionBehavior.ThrowOnSmallDenominator; // throws on small denominator

try
{
    var r = UDouble.Divide(a, b);
}
catch (DivideByZeroException)
{
    // expected when |b.Mean| <= DivisionTolerance
}
```

2) SaturateVariance

```csharp
Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior =
    Uncertainty.Core.Policies.DivisionBehavior.SaturateVariance; // return finite result with saturated variance

var r = UDouble.Divide(a, b);
// r.Variance will be <= VarianceSaturationPolicy.AbsoluteVarianceMax in edge cases
```

3) ReturnInfinityMean

```csharp
Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior =
    Uncertainty.Core.Policies.DivisionBehavior.ReturnInfinityMean; // intent: ±Infinity mean

var r = UDouble.Divide(a, b);
// Note: current implementation treats this as a numeric-robustness policy and returns a finite
// (saturated) result to preserve the library contract that means are finite. This may be
// changed in future releases if we relax core contracts.
```

I also added an example `ClampingDivisionStrategy` in `examples/` which clamps the denominator to ±DivisionTolerance to produce deterministic, finite results.

## Changing the division tolerance

The tolerance is now configured via a setter method to make the intent explicit and reduce accidental assignments:

```csharp
// read current value
double tol = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionTolerance;

// set a new finite non-negative tolerance
Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
```

> Note: `UDouble.DivisionTolerance` is obsolete and forwards to `SetDivisionTolerance` for backwards compatibility.

## Summary

- Default values: `DivisionTolerance = 0.0`, `DivisionBehavior = ThrowOnSmallDenominator`.
- Use `SetDivisionTolerance(double)` to configure tolerance and `DivisionStrategy`/`DivisionBehavior` to change behaviour.

If you'd like, I can add this file to the README or reference it from `docs/STRUCTURE.md` so it becomes discoverable in docs navigation.