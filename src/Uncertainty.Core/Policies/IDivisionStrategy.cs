using Uncertainty.Core;

namespace Uncertainty.Core.Policies
{
    /// <summary>
    /// Strategy interface for handling division when the denominator mean is considered "small".
    /// Implementations decide whether to throw, saturate variance, or return an infinite mean, etc.
    /// </summary>
    public interface IDivisionStrategy
    {
        /// <summary>
        /// Handle a division where the denominator mean is within the configured tolerance.
        /// Implementations may throw or return a finite or infinite <see cref="UDouble"/>.
        /// </summary>
        UDouble HandleSmallDenominator(UDouble a, UDouble b);
    }
}
