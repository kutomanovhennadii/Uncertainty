using Uncertainty.Core.Policies;

namespace Uncertainty.Core.Policies.DivisionPolicies.DivisionStrategies
{
    /// <summary>
    /// Internal marker interface implemented by built-in division strategies
    /// to expose their corresponding <see cref="DivisionBehavior"/> mapping.
    /// </summary>
    internal interface IMappedDivisionBehavior
    {
        DivisionBehavior MappedBehavior { get; }
    }
}
