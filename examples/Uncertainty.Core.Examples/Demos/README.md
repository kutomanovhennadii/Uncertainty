# Division Strategy Demos

This folder contains runnable demos that showcase built-in and custom division strategies used
by `Uncertainty.Core` when a denominator mean is considered "small" (|mean| ≤ `DivisionTolerance`).

Files
- `DivisionStrategyDemos.cs` — demo runner that executes all demos sequentially.
- `ClampingDivisionStrategy.cs` — example custom strategy that clamps the denominator to ±`DivisionTolerance`
  and returns a finite result.

What the demos show
1. ThrowOnSmallDenominator (default)
   - Behavior: throws `DivideByZeroException` when |denominator.Mean| ≤ `DivisionTolerance`.
2. SaturateVariance
   - Behavior: performs division and then saturates the computed variance to a finite ceiling (numeric
     robustness, avoids NaN/∞ variance).
3. ReturnInfinityMean
   - Behavior: intended to signal a near-singularity by ±Infinity mean. Currently the implementation falls
     back to the saturated behavior to preserve the library contract (means remain finite).
4. Custom: ClampingDivisionStrategy (illustration)
   - Behavior: included as an internal illustration. Clients should not assign custom strategies directly; use `DivisionBehavior` instead (e.g. `SaturateVariance`) to select built-in behavior.

How to run
- Run the example project which invokes all demos:

```bash
dotnet run --project examples/Uncertainty.Core.Examples/Uncertainty.Core.Examples.csproj
```

- Or run / debug from your IDE (open `examples/Uncertainty.Core.Examples` and run `Program.cs`).

Configuration notes
- Adjust `DivisionTolerance` via the policy API:

```csharp
Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
```

- Switch built-in behaviors via:

```csharp
Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = Uncertainty.Core.Policies.DivisionBehavior.SaturateVariance;
```

- Or install a custom strategy in code:

```csharp
Uncertainty.Core.Policies.UncertaintyPolicies.DivisionStrategy = new ClampingDivisionStrategy();
```

Testing
- There are unit tests in `tests/Uncertainty.Core.Tests/DivisionBehaviorTests.cs` that exercise the built-in behaviors.

Notes
- The demos are intentionally simple and focused on demonstrating the behavioral differences.
  For production code, implement strategies that align with your statistical model and numeric expectations.
