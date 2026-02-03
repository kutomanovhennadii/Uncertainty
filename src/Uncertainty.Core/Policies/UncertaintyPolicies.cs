namespace Uncertainty.Core.Policies
{
    /// <summary>
    /// Policies for numeric behavior in the Uncertainty.Core arithmetic operations.
    /// </summary>
    public static class UncertaintyPolicies
    {
        /// <summary>
        /// Specifies how division by a very small denominator should behave.
        /// Default is <see cref="DivisionBehavior.ThrowOnSmallDenominator"/> to preserve
        /// backward compatibility.
        /// </summary>
        public static DivisionBehavior DivisionBehavior { get; set; } = DivisionBehavior.ThrowOnSmallDenominator;
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
        /// Intended to return +/-Infinity as the mean when the denominator is very small. Not implemented specially
        /// yet; currently maps to <see cref="SaturateVariance"/> as a fallback.
        /// </summary>
        ReturnInfinityMean
    }
}