using NUnit.Framework;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    public sealed class DivisionBehaviorTests
    {
        [Test]
        public void ThrowOnSmallDenominator_Throws()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            double prevTol = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionTolerance;
            var prevPolicy = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior;
            try
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(1e-307);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = Uncertainty.Core.Policies.DivisionBehavior.ThrowOnSmallDenominator;

                Assert.That(() => UDouble.Divide(a, b), Throws.TypeOf<DivideByZeroException>());
            }
            finally
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(prevTol);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        [Test]
        public void SaturateVariance_ReturnsFinite()
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
                Assert.That(r.Variance, Is.LessThanOrEqualTo(1e300));
            }
            finally
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(prevTol);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        [Test]
        public void ReturnInfinityMean_FallbackToSaturate_ReturnsFinite()
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

                Assert.That(!double.IsNaN(r.Mean), Is.True);
                Assert.That(double.IsFinite(r.Variance), Is.True);
                Assert.That(r.Variance, Is.LessThanOrEqualTo(1e300));
            }
            finally
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(prevTol);
                Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        // Direct assignment of a custom DivisionStrategy is not supported for library consumers.
        // Use UncertaintyPolicies.DivisionBehavior to select built-in behavior (ThrowOnSmallDenominator, SaturateVariance, ReturnInfinityMean).

        [Test]
        public void Defaults_AreExpected()
        {
            // Defaults are set in the static constructor of UncertaintyPolicies.
            Assert.That(Uncertainty.Core.Policies.UncertaintyPolicies.DivisionTolerance, Is.EqualTo(0.0));
            Assert.That(Uncertainty.Core.Policies.UncertaintyPolicies.DivisionBehavior, Is.EqualTo(Uncertainty.Core.Policies.DivisionBehavior.ThrowOnSmallDenominator));
            Assert.That(Uncertainty.Core.Policies.UncertaintyPolicies.DivisionStrategy, Is.Not.Null);
        }

        [Test]
        public void SetDivisionTolerance_InvalidValues_Throw()
        {
            double prev = Uncertainty.Core.Policies.UncertaintyPolicies.DivisionTolerance;
            try
            {
                Assert.That(() => Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(double.NaN), Throws.TypeOf<ArgumentException>());
                Assert.That(() => Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(double.PositiveInfinity), Throws.TypeOf<ArgumentException>());
                Assert.That(() => Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(-1.0), Throws.TypeOf<ArgumentOutOfRangeException>());
            }
            finally
            {
                Uncertainty.Core.Policies.UncertaintyPolicies.SetDivisionTolerance(prev);
            }
        }
    }
}
