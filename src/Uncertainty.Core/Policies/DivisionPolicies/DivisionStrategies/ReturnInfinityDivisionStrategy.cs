using Uncertainty.Core;
using Uncertainty.Core.Policies.DivisionPolicies;
using Uncertainty.Core.Policies.VarianceSaturationPolicies;

namespace Uncertainty.Core.Policies.DivisionPolicies.DivisionStrategies
{
    /// <summary>
    /// Policy that conceptually allows infinite means but currently defers to saturation for contract safety.
    /// </summary>
    internal sealed class ReturnInfinityDivisionStrategy : IDivisionStrategy, IMappedDivisionBehavior
    {
        /// <summary>
        /// Conceptually returns infinite mean, but currently applies variance saturation to maintain contract compliance.
        /// </summary>
        /// <param name="a">Numerator.</param>
        /// <param name="b">Denominator with small mean.</param>
        /// <returns>Division result with saturated variance (may change in future to allow infinite means).</returns>
        public UDouble HandleSmallDenominator(UDouble a, UDouble b)
        {
            // Returning ±Infinity violates the current contract, so fall back to variance saturation.
            double mean = a.Mean / b.Mean;

            double b2 = b.Mean * b.Mean;
            double b4 = b2 * b2;

            double variance =
                a.Variance / b2 +
                (a.Mean * a.Mean * b.Variance) / b4;

            variance = VarianceSaturationPolicy.SaturateVariance(mean, variance);

            return UDouble.FromMeanVar(mean, variance);
        }

        DivisionBehavior IMappedDivisionBehavior.MappedBehavior => DivisionBehavior.ReturnInfinityMean;
    }
}
