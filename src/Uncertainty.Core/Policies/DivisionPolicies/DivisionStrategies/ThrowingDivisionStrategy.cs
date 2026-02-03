using System;
using Uncertainty.Core;
using Uncertainty.Core.Policies.DivisionPolicies;

namespace Uncertainty.Core.Policies.DivisionPolicies.DivisionStrategies
{
    /// <summary>
    /// Policy that fails fast by throwing when the denominator mean is below the configured threshold.
    /// </summary>
    internal sealed class ThrowingDivisionStrategy : IDivisionStrategy, IMappedDivisionBehavior
    {
        /// <summary>
        /// Throws <see cref="DivideByZeroException"/> unconditionally.
        /// </summary>
        /// <param name="a">Numerator (unused).</param>
        /// <param name="b">Denominator with small mean (unused).</param>
        /// <returns>Never returns; always throws.</returns>
        /// <exception cref="DivideByZeroException">Always thrown.</exception>
        public UDouble HandleSmallDenominator(UDouble a, UDouble b)
        {
            throw new DivideByZeroException("Denominator mean is too close to zero.");
        }

        DivisionBehavior IMappedDivisionBehavior.MappedBehavior => DivisionBehavior.ThrowOnSmallDenominator;
    }
}
