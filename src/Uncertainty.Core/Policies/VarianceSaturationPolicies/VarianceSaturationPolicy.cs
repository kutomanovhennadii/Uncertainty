using System;
using Uncertainty.Core.Policies.VarianceSaturationPolicies;
using Uncertainty.Core.Policies;

namespace Uncertainty.Core.Policies.VarianceSaturationPolicies
{
    /// <summary>
    /// Numeric-stability policy for saturating <c>Variance</c> in <c>UDouble</c>.
    /// Moved into Policies/VarianceSaturationPolicies and now reads runtime options from <see cref="UncertaintyPolicies"/>.
    /// </summary>
    internal static class VarianceSaturationPolicy
    {
        /// <summary>
        /// Saturates a computed variance according to the numeric-stability policy.
        /// This implementation uses values provided via <see cref="UncertaintyPolicies.VarianceSaturation"/>.
        /// </summary>
        internal static double SaturateVariance(double mean, double variance)
        {
            var opts = UncertaintyPolicies.VarianceSaturation;

            double absMean = Math.Abs(mean);
            double relLimit;

            if (double.IsFinite(absMean))
            {
                double meanSq = absMean * absMean;
                // compute factor = MaxRelativeStdDev^2 but guard against overflow
                double maxRelFactor = opts.MaxRelativeStdDev * opts.MaxRelativeStdDev;
                relLimit = meanSq * maxRelFactor;

                if (!double.IsFinite(relLimit) || relLimit <= 0.0)
                {
                    relLimit = opts.AbsoluteVarianceMax;
                }
            }
            else
            {
                relLimit = opts.AbsoluteVarianceMax;
            }

            double ceiling = Math.Max(relLimit, opts.AbsoluteVarianceMax);

            if (!double.IsFinite(variance) || variance > ceiling)
                return ceiling;

            return variance;
        }
    }
}
