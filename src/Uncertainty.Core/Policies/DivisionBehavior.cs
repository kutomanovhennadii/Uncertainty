namespace Uncertainty.Core.Policies
{
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
