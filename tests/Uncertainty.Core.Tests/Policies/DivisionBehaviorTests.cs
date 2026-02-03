using NUnit.Framework;
using System;
using Uncertainty.Core;
using Uncertainty.Core.Policies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    public sealed class DivisionBehaviorTests
    {
        [Test]
        public void ThrowOnSmallDenominator_Throws()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            double prevTol = UncertaintyPolicies.DivisionTolerance;
            var prevPolicy = UncertaintyPolicies.DivisionBehavior;
            try
            {
                UncertaintyPolicies.SetDivisionTolerance(1e-307);
                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;

                Assert.That(() => UDouble.Divide(a, b), Throws.TypeOf<DivideByZeroException>());
            }
            finally
            {
                UncertaintyPolicies.SetDivisionTolerance(prevTol);
                UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        [Test]
        public void SaturateVariance_ReturnsFinite()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            double prevTol = UncertaintyPolicies.DivisionTolerance;
            var prevPolicy = UncertaintyPolicies.DivisionBehavior;
            try
            {
                UncertaintyPolicies.SetDivisionTolerance(1e-307);
                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.SaturateVariance;

                var r = UDouble.Divide(a, b);

                Assert.That(double.IsFinite(r.Variance), Is.True);
                Assert.That(r.Variance, Is.LessThanOrEqualTo(1e300));
            }
            finally
            {
                UncertaintyPolicies.SetDivisionTolerance(prevTol);
                UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        [Test]
        public void ReturnInfinityMean_FallbackToSaturate_ReturnsFinite()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            double prevTol = UncertaintyPolicies.DivisionTolerance;
            var prevPolicy = UncertaintyPolicies.DivisionBehavior;
            try
            {
                UncertaintyPolicies.SetDivisionTolerance(1e-307);
                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ReturnInfinityMean;

                var r = UDouble.Divide(a, b);

                Assert.That(!double.IsNaN(r.Mean), Is.True);
                Assert.That(double.IsFinite(r.Variance), Is.True);
                Assert.That(r.Variance, Is.LessThanOrEqualTo(1e300));
            }
            finally
            {
                UncertaintyPolicies.SetDivisionTolerance(prevTol);
                UncertaintyPolicies.DivisionBehavior = prevPolicy;
            }
        }

        [Test]
        public void Defaults_AreExpected()
        {
            Assert.That(UncertaintyPolicies.DivisionTolerance, Is.EqualTo(0.0));
            Assert.That(UncertaintyPolicies.DivisionBehavior, Is.EqualTo(DivisionBehavior.ThrowOnSmallDenominator));
            Assert.That(UncertaintyPolicies.DivisionStrategy, Is.Not.Null);
        }

        [Test]
        public void SetDivisionTolerance_InvalidValues_Throw()
        {
            double prev = UncertaintyPolicies.DivisionTolerance;
            try
            {
                Assert.That(() => UncertaintyPolicies.SetDivisionTolerance(double.NaN), Throws.TypeOf<ArgumentException>());
                Assert.That(() => UncertaintyPolicies.SetDivisionTolerance(double.PositiveInfinity), Throws.TypeOf<ArgumentException>());
                Assert.That(() => UncertaintyPolicies.SetDivisionTolerance(-1.0), Throws.TypeOf<ArgumentOutOfRangeException>());
            }
            finally
            {
                UncertaintyPolicies.SetDivisionTolerance(prev);
            }
        }
    }
}
