using System;

namespace Uncertainty.Core.Policies.VarianceSaturationPolicies
{
    /// <summary>
    /// Immutable set of parameters for the variance saturation policy.
    /// </summary>
    public readonly record struct VarianceSaturationOptions(double MaxRelativeStdDev, double AbsoluteVarianceMax)
    {
        /// <summary>
        /// Current contract default for the relative standard deviation cap.
        /// </summary>
        public const double DefaultMaxRelativeStdDev = 1e8;

        /// <summary>
        /// Minimum absolute variance ceiling mandated by the contract.
        /// </summary>
        public const double DefaultAbsoluteVarianceMax = 1e300;

        /// <summary>
        /// Default options matching the existing VarianceSaturationPolicy implementation.
        /// </summary>
        public static VarianceSaturationOptions Default { get; } = new(DefaultMaxRelativeStdDev, DefaultAbsoluteVarianceMax);

        /// <summary>
        /// Validates values and throws when constraints are violated.
        /// </summary>
        public void EnsureValid()
        {
            if (!double.IsFinite(MaxRelativeStdDev) || MaxRelativeStdDev < 1.0)
                throw new ArgumentOutOfRangeException(nameof(MaxRelativeStdDev), MaxRelativeStdDev, "MaxRelativeStdDev must be finite and >= 1.");

            if (!double.IsFinite(AbsoluteVarianceMax) || AbsoluteVarianceMax < DefaultAbsoluteVarianceMax)
                throw new ArgumentOutOfRangeException(nameof(AbsoluteVarianceMax), AbsoluteVarianceMax, $"AbsoluteVarianceMax must be finite and >= {DefaultAbsoluteVarianceMax:G}.");
        }

        /// <summary>
        /// Tries to validate values without throwing exceptions.
        /// </summary>
        public bool TryValidate(out string? error)
        {
            try
            {
                EnsureValid();
                error = null;
                return true;
            }
            catch (ArgumentOutOfRangeException ex)
            {
                error = ex.Message;
                return false;
            }
        }
    }
}
