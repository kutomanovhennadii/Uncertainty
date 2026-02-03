using System;

namespace Uncertainty.Core.Policies
{
    /// <summary>
    /// Policies for numeric behavior in the Uncertainty.Core arithmetic operations.
    /// </summary>
    public static class UncertaintyPolicies
    {
        private static DivisionBehavior _divisionBehavior;
        private static IDivisionStrategy? _divisionStrategy;

        static UncertaintyPolicies()
        {
            // Explicitly set defaults for clarity and to avoid relying on implicit language defaults.
            DivisionTolerance = 0.0;
            DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;
        }

        /// <summary>
        /// Tolerance used to treat small denominators as zero in division operations.
        /// Default is 0.0 to preserve exact behavior (<c>b.Mean == 0.0</c>).
        /// Use <see cref="SetDivisionTolerance(double)"/> to modify this value.
        /// </summary>
        public static double DivisionTolerance { get; private set; }

        /// <summary>
        /// Sets the division tolerance. Must be finite and &gt;= 0.
        /// </summary>
        public static void SetDivisionTolerance(double tolerance)
        {
            if (double.IsNaN(tolerance) || double.IsInfinity(tolerance))
                throw new ArgumentException("Division tolerance must be finite.", nameof(tolerance));
            if (tolerance < 0.0)
                throw new ArgumentOutOfRangeException(nameof(tolerance), "Division tolerance must be ≥ 0.");
            DivisionTolerance = tolerance;
        }

        /// <summary>
        /// Specifies how division by a very small denominator should behave.
        /// Setting this will also update the <see cref="DivisionStrategy"/> to a default implementation.
        /// </summary>
        public static DivisionBehavior DivisionBehavior
        {
            get => _divisionBehavior;
            set
            {
                _divisionBehavior = value;

                // Map enum to default strategy implementations.
                _divisionStrategy = value switch
                {
                    DivisionBehavior.ThrowOnSmallDenominator => new ThrowingDivisionStrategy(),
                    DivisionBehavior.SaturateVariance => new SaturatingDivisionStrategy(),
                    DivisionBehavior.ReturnInfinityMean => new ReturnInfinityDivisionStrategy(),
                    _ => new ThrowingDivisionStrategy()
                };
            }
        }

        /// <summary>
        /// Division strategy used when a denominator is considered "small" according to <see cref="DivisionTolerance"/>.
        /// Assigning a custom strategy updates the <see cref="DivisionBehavior"/> to a best-effort value for compatibility.
        /// </summary>
        public static IDivisionStrategy DivisionStrategy
        {
            get => _divisionStrategy ??= new ThrowingDivisionStrategy();
            internal set
            {
                _divisionStrategy = value ?? throw new ArgumentNullException(nameof(value));

                // Update enum for backward compatibility when possible.
                switch (value)
                {
                    case ThrowingDivisionStrategy:
                        _divisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;
                        break;
                    case SaturatingDivisionStrategy:
                        _divisionBehavior = DivisionBehavior.SaturateVariance;
                        break;
                    case ReturnInfinityDivisionStrategy:
                        _divisionBehavior = DivisionBehavior.ReturnInfinityMean;
                        break;
                    default:
                        _divisionBehavior = DivisionBehavior.SaturateVariance;
                        break;
                }
            }
        }
    }

    /// <summary>
    /// Behavior choices for division where the denominator mean is near zero.
    /// </summary>
    public enum DivisionBehavior
    {
        /// <summary>
        /// The current conservative behavior: treat denominators with |mean| ≤ DivisionTolerance as zero and throw.
        /// </summary>
        ThrowOnSmallDenominator,

        /// <summary>
        /// Perform the division and then apply the variance saturation policy so that a finite UDouble is returned.
        /// </summary>
        SaturateVariance,

        /// <summary>
        /// Intended to return +/-Infinity as the mean when the denominator is very small.
        /// </summary>
        ReturnInfinityMean
    }
}