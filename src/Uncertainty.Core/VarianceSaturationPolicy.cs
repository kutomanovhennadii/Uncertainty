using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Uncertainty.Core
{
    /// <summary>
    /// Numeric-stability policy for saturating <c>Variance</c> in <c>UDouble</c>.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <b>Scope.</b> This policy is a numerical safeguard, not a statistical model.
    /// It exists to keep <c>Variance</c> finite and bounded when linear error propagation
    /// becomes numerically unstable (for example, near singularities such as division by a small mean).
    /// </para>
    /// <para>
    /// <b>Why saturation is needed.</b> The core model uses first-order (linear) propagation,
    /// which is only valid in the “small error” regime. When the computed variance grows too large,
    /// it no longer represents a meaningful uncertainty estimate and can explode to <c>NaN</c> or <c>Infinity</c>,
    /// breaking downstream computations. This policy clamps such values to a ceiling that is:
    /// (1) scale-aware (depends on <c>mean</c>) and (2) has an absolute lower bound mandated by the contract.
    /// </para>
    /// <para>
    /// <b>Contract constraints.</b>
    /// The implementation must define an upper ceiling for variance and it must be at least <c>1e300</c>
    /// (see <c>core-signatures.md</c>), and the representation must not produce <c>NaN</c>/<c>Infinity</c>
    /// for <c>Variance</c> in valid values (see core contracts).
    /// </para>
    /// <para>
    /// <b>Design choice: relative ceiling.</b>
    /// We cap the <i>relative standard deviation</i>:
    /// <c>StdDev / |Mean| &lt;= MaxRelativeStdDev</c>.
    /// This yields a derived variance ceiling:
    /// <c>Variance &lt;= (MaxRelativeStdDev^2) * Mean^2</c>.
    /// The chosen <see cref="MaxRelativeStdDev"/> is an engineering threshold for “still in the small-error regime”.
    /// </para>
    /// <para>
    /// <b>Final ceiling.</b>
    /// The effective ceiling is:
    /// <c>ceiling = max(Mean^2 * MaxRelativeVarianceFactor, AbsoluteVarianceMax)</c>.
    /// If the computed variance is not finite or exceeds this ceiling, we return <c>ceiling</c>.
    /// Otherwise we return the input variance unchanged.
    /// </para>
    /// <para>
    /// <b>Non-goals.</b>
    /// This policy does not modify <c>Mean</c>, does not attempt to correct negative variance,
    /// and does not handle domain errors (those are addressed elsewhere by construction rules).
    /// </para>
    /// </remarks>
    internal static class VarianceSaturationPolicy
    {
        /// <summary>
        /// Absolute lower bound for the variance ceiling required by the core contract.
        /// </summary>
        /// <remarks>
        /// The contract requires that the implementation-defined maximum variance ceiling is
        /// at least <c>1e300</c>. This value acts as a floor for the effective ceiling,
        /// especially when <c>Mean</c> is near zero.
        /// </remarks>
        internal const double AbsoluteVarianceMax = 1e300;

        /// <summary>
        /// Maximum allowed relative standard deviation (<c>StdDev / |Mean|</c>) in the “small-error” regime.
        /// </summary>
        /// <remarks>
        /// This is an engineering threshold used to detect when linear propagation becomes meaningless.
        /// It is not a probabilistic guarantee; it controls the numeric stability envelope.
        /// </remarks>
        internal const double MaxRelativeStdDev = 1e8;

        /// <summary>
        /// Derived factor for the relative variance ceiling: <c>MaxRelativeStdDev^2</c>.
        /// </summary>
        /// <remarks>
        /// Used to compute the scale-aware limit: <c>Mean^2 * MaxRelativeVarianceFactor</c>.
        /// </remarks>
        internal const double MaxRelativeVarianceFactor = MaxRelativeStdDev * MaxRelativeStdDev; // 1e16

        /// <summary>
        /// Saturates a computed variance according to the numeric-stability policy.
        /// </summary>
        /// <param name="mean">
        /// Mean of the value for which the variance was computed. Used only to set a scale-aware ceiling.
        /// </param>
        /// <param name="variance">
        /// Computed variance to be checked and possibly saturated.
        /// </param>
        /// <returns>
        /// The original <paramref name="variance"/> if it is finite and does not exceed the effective ceiling;
        /// otherwise, the effective ceiling value.
        /// </returns>
        /// <remarks>
        /// Effective ceiling:
        /// <c>ceiling = max(mean^2 * MaxRelativeVarianceFactor, AbsoluteVarianceMax)</c>.
        /// Saturation triggers when:
        /// <c>variance</c> is <c>NaN</c>, <c>Infinity</c>, or <c>variance &gt; ceiling</c>.
        /// </remarks>
        internal static double SaturateVariance(double mean, double variance)
        {
            double relLimit = mean * mean * MaxRelativeVarianceFactor;
            double ceiling = Math.Max(relLimit, AbsoluteVarianceMax);

            if (!double.IsFinite(variance) || variance > ceiling)
                return ceiling;

            return variance;
        }
    }


}
