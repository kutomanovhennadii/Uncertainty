# Changelog

## [Unreleased]
- Initial project structure
- Unify argument exception behavior: use ArgumentException for non-finite inputs and ArgumentOutOfRangeException for numeric range violations (negative variance/stddev)
- Add Welford one-pass aggregation and improved tests for statistical stability
- Add VarianceSaturationPolicy unit tests and apply saturation consistently to arithmetic operators
- Add configurable DivisionTolerance and tests covering division edge-cases
- Add culture matrix CI job to validate formatting behavior
