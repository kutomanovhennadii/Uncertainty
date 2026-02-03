using Uncertainty.Core;

namespace Uncertainty.Core.Policies.DivisionPolicies
{
    /// <summary>
    /// Strategy interface for handling division when the denominator mean is considered "small".
    /// This interface is internal; custom strategies are not supported.
    /// Users should configure behavior via the <see cref="DivisionBehavior"/> enum.
    /// </summary>
    internal interface IDivisionStrategy
    {
        /// <summary>
        /// Handle a division where the denominator mean is within the configured tolerance.
        /// Implementations may throw or return a finite or infinite <see cref="UDouble"/>.
        /// </summary>
        /// <param name="a">Numerator.</param>
        /// <param name="b">Denominator with mean considered "small".</param>
        /// <returns>Result of the division operation, behavior depends on implementation.</returns>
        UDouble HandleSmallDenominator(UDouble a, UDouble b);
    }
}
