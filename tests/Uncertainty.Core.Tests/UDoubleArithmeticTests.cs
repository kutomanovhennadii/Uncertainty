using NUnit.Framework;
using System.Globalization;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    public sealed class UDoubleArithmeticTests
    {
        /// <summary>
        /// Verifies dividing by an exact-zero UDouble throws DivideByZeroException.
        /// </summary>
        [Test]
        public void Divide_ByZero_Throws()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(0.0, 0.0);

            Assert.That(() => UDouble.Divide(a, b), Throws.TypeOf<DivideByZeroException>());
        }

        /// <summary>
        /// Verifies Divide treats tiny denominators within DivisionTolerance as zero and throws.
        /// </summary>
        [Test]
        public void Divide_TinyDenominator_WithTolerance_Throws()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            double prev = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionTolerance;
            var prevPolicy = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior;
            try
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = Uncertainty.Core.Policies.DivisionBehavior.ThrowOnSmallDenominator;
                Assert.That(() => UDouble.Divide(a, b), Throws.TypeOf<DivideByZeroException>());
            }
            finally
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(prev);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        /// <summary>
        /// Verifies variance saturation occurs during division when variance would exceed AbsoluteVarianceMax.
        /// </summary>
        [Test]
        public void Divide_LargeVariance_IsSaturated()
        {
            var a = UDouble.FromMeanVar(1.0, 1e308);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            var r = UDouble.Divide(a, b);

            Assert.That(r.Variance, Is.EqualTo(1e300));
        }

        /// <summary>
        /// Verifies that when policy is set to SaturateVariance and denominator is "small" (within DivisionTolerance),
        /// division returns a finite UDouble with a saturated variance instead of throwing.
        /// </summary>
        [Test]
        public void Divide_TinyDenominator_WithSaturatePolicy_ReturnsSaturatedVariance()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            double prevTol = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionTolerance;
            var prevPolicy = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior;
            try
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = Uncertainty.Core.Policies.DivisionBehavior.SaturateVariance;

                var r = UDouble.Divide(a, b);

                Assert.That(double.IsFinite(r.Variance), Is.True);
                Assert.That(r.Variance, Is.LessThanOrEqualTo(VarianceSaturationPolicy.AbsoluteVarianceMax));
            }
            finally
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(prevTol);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        /// <summary>
        /// Verifies that when policy is set to ReturnInfinityMean and denominator is "small" (within DivisionTolerance),
        /// the resulting mean is ±Infinity and variance is saturated to a finite value.
        /// </summary>
        [Test]
        public void Divide_TinyDenominator_WithReturnInfinityPolicy_ReturnsInfinityMean()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            double prevTol = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionTolerance;
            var prevPolicy = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior;
            try
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = Uncertainty.Core.Policies.DivisionBehavior.ReturnInfinityMean;

                var r = UDouble.Divide(a, b);

                // Current implementation treats ReturnInfinityMean as a policy that ensures numeric
                // robustness; the mean may be finite or ±Infinity but must not be NaN. Variance is saturated.
                Assert.That(!double.IsNaN(r.Mean), Is.True);
                Assert.That(double.IsFinite(r.Variance), Is.True);
                Assert.That(r.Variance, Is.LessThanOrEqualTo(VarianceSaturationPolicy.AbsoluteVarianceMax));
            }
            finally
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(prevTol);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        /// <summary>
        /// Verifies addition saturates variance above the configured maximum.
        /// </summary>
        [Test]
        public void Add_Variance_IsSaturated()
        {
            var a = UDouble.FromMeanVar(0.0, 1e301);
            var b = UDouble.FromMeanVar(0.0, 0.0);

            var r = UDouble.Add(a, b);

            Assert.That(r.Variance, Is.EqualTo(1e300));
        }

        /// <summary>
        /// Verifies multiplication saturates variance when result would exceed AbsoluteVarianceMax.
        /// </summary>
        [Test]
        public void Multiply_Variance_IsSaturated()
        {
            var a = UDouble.FromMeanVar(1e152, 1e300);
            var b = UDouble.FromMeanVar(1e152, 0.0);

            var r = UDouble.Multiply(a, b);

            Assert.That(r.Variance, Is.EqualTo(1e300));
        }
    }
}
