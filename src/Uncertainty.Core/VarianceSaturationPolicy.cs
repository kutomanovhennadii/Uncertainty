using System;
using Uncertainty.Core.Policies.VarianceSaturationPolicies;
using Uncertainty.Core.Policies;

namespace Uncertainty.Core
{
    /// <summary>
    /// Numeric-stability policy for saturating <c>Variance</c> in <c>UDouble</c>.
    /// This is a legacy wrapper that delegates to the new implementation in <see cref="Uncertainty.Core.Policies.VarianceSaturationPolicies.VarianceSaturationPolicy"/>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Scope.</b> This policy is a numerical safeguard, not a statistical model.
    /// It exists to keep <c>Variance</c> finite and bounded when linear error propagation
    /// becomes numerically unstable (for example, near singularities such as division by a small mean).
    /// </para>
    /// <para>
    /// <b>Design choice: relative ceiling.</b>
    /// We cap the <i>relative standard deviation</i>:
    /// <c>StdDev / |Mean| &lt;= MaxRelativeStdDev</c>.
    /// This yields a derived variance ceiling:
    /// <c>Variance &lt;= (MaxRelativeStdDev^2) * Mean^2</c>.
    /// </para>
    /// </remarks>
    internal static class VarianceSaturationPolicy
    {
        /// <summary>
        /// Absolute lower bound for the variance ceiling required by the core contract.
        /// </summary>
        internal const double AbsoluteVarianceMax = 1e300;

        /// <summary>
        /// Maximum allowed relative standard deviation (<c>StdDev / |Mean|</c>) in the "small-error" regime.
        /// </summary>
        internal const double MaxRelativeStdDev = 1e8;

        /// <summary>
        /// Derived factor for the relative variance ceiling: <c>MaxRelativeStdDev^2</c>.
        /// </summary>
        internal const double MaxRelativeVarianceFactor = MaxRelativeStdDev * MaxRelativeStdDev; // 1e16

        /// <summary>
        /// Saturates a computed variance according to the numeric-stability policy.
        /// Delegates to <see cref="Uncertainty.Core.Policies.VarianceSaturationPolicies.VarianceSaturationPolicy.SaturateVariance(double, double)"/>.
        /// </summary>
        /// <param name="mean">Mean of the value for which the variance was computed.</param>
        /// <param name="variance">Computed variance to be checked and possibly saturated.</param>
        /// <returns>The original variance if finite and not exceeding ceiling; otherwise, the ceiling value.</returns>
        internal static double SaturateVariance(double mean, double variance)
        {
            // Delegate to the new implementation in Policies/VarianceSaturationPolicies
            return Uncertainty.Core.Policies.VarianceSaturationPolicies.VarianceSaturationPolicy.SaturateVariance(mean, variance);
        }
    }
}
