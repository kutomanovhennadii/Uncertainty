Title: Unify exception policy, add DivisionTolerance, Welford aggregation, saturation tests, and CI culture matrix

Summary:
- Implement unified exception policy across core types:
  - Use ArgumentException for NaN/Infinity (non-finite) checks.
  - Use ArgumentOutOfRangeException for numeric range violations (e.g., negative variance or stdDev).
- Add Welford one-pass aggregation in `UDouble.FromData` to improve numerical stability and avoid allocations.
- Add `UDouble.Zero` constant and `ToString`/`IFormattable` improvements.
- Add `UDouble.DivisionTolerance` and use it to detect denominators that are effectively zero.
- Apply `VarianceSaturationPolicy.SaturateVariance` consistently across Add/Subtract/Multiply/Divide.
- Add direct unit tests for `VarianceSaturationPolicy` and additional tests for subnormals, Welford stability, `ToString` formatting, and DivisionTolerance behavior.
- Add CI job `test-cultures` to run tests under several locales to validate formatting behavior.

Files changed (high level):
- src/Uncertainty.Core/UDouble.cs
- src/Uncertainty.Core/VarianceSaturationPolicy.cs
- src/Uncertainty.Core/InternalsVisibleTo.cs (new)
- tests/Uncertainty.Core.Tests/UDoubleFactoryTests.cs (updated/expanded)
- tests/Uncertainty.Core.Tests/VarianceSaturationPolicyTests.cs (new)
- .github/workflows/ci.yml (CI: added culture matrix)
- CHANGELOG.md, docs/Contracts/conversions-and-comparisons.md (docs updates)

Notes for reviewers:
- Policy decision: ArgumentException for non-finite inputs; ArgumentOutOfRangeException for numeric range violations. Please confirm if acceptable across the broader project.
- DivisionTolerance default is 0.0; intended as opt-in tuning by callers. Consider adding a policy/enum if more behavior is desired (Throw/Saturate/Infinity).

Testing:
- All unit tests pass locally: `dotnet test` (51 tests currently).
- Recommend running CI to validate the new matrix and any platform-specific behavior.

If accepted, I will squash commits into a clean change set or split into smaller PRs if preferred.