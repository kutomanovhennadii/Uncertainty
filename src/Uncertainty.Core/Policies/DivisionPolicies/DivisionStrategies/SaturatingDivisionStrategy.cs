using Uncertainty.Core;
using Uncertainty.Core.Policies.DivisionPolicies;
using Uncertainty.Core.Policies.VarianceSaturationPolicies;

namespace Uncertainty.Core.Policies.DivisionPolicies.DivisionStrategies
{
    /// <summary>
    /// Policy that performs the division and clamps the resulting variance using the saturation policy.
    /// </summary>
    internal sealed class SaturatingDivisionStrategy : IDivisionStrategy, IMappedDivisionBehavior
    {
        /// <summary>
        /// Performs division and saturates the resulting variance to ensure finite output.
        /// </summary>
        /// <param name="a">Numerator.</param>
        /// <param name="b">Denominator with small mean.</param>
        /// <returns>Division result with saturated variance.</returns>
        public UDouble HandleSmallDenominator(UDouble a, UDouble b)
        {
            double mean = a.Mean / b.Mean;

            double b2 = b.Mean * b.Mean;
            double b4 = b2 * b2;

            double variance =
                a.Variance / b2 +
                (a.Mean * a.Mean * b.Variance) / b4;

            variance = VarianceSaturationPolicy.SaturateVariance(mean, variance);

            return UDouble.FromMeanVar(mean, variance);
        }

        DivisionBehavior IMappedDivisionBehavior.MappedBehavior => DivisionBehavior.SaturateVariance;
    }
}
