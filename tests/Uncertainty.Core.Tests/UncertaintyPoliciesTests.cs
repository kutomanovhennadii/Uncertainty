using NUnit.Framework;
using System;
using Uncertainty.Core.Policies;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    /// <summary>
    /// Validates policy defaults, mappings, and tolerance guardrails.
    /// </summary>
    public sealed class UncertaintyPoliciesTests
    {
        /// <summary>
        /// Defaults should match ThrowOnSmallDenominator with a throwing strategy.
        /// </summary>
        [Test]
        public void Defaults_AreExpected()
        {
            Assert.That(UncertaintyPolicies.DivisionTolerance, Is.EqualTo(0.0));
            Assert.That(UncertaintyPolicies.DivisionBehavior, Is.EqualTo(DivisionBehavior.ThrowOnSmallDenominator));
            Assert.That(UncertaintyPolicies.DivisionStrategy, Is.Not.Null);
            Assert.That(UncertaintyPolicies.DivisionStrategy.GetType().Name, Is.EqualTo("ThrowingDivisionStrategy"));
        }

        [TestCase(DivisionBehavior.ThrowOnSmallDenominator, "ThrowingDivisionStrategy")]
        [TestCase(DivisionBehavior.SaturateVariance, "SaturatingDivisionStrategy")]
        [TestCase(DivisionBehavior.ReturnInfinityMean, "ReturnInfinityDivisionStrategy")]
        /// <summary>
        /// Each enum value must map to its corresponding built-in strategy.
        /// </summary>
        public void DivisionBehavior_MapsToExpectedStrategy(DivisionBehavior behavior, string expectedTypeName)
        {
            var prev = UncertaintyPolicies.DivisionBehavior;
            try
            {
                UncertaintyPolicies.DivisionBehavior = behavior;

                var strategy = UncertaintyPolicies.DivisionStrategy;
                Assert.That(strategy, Is.Not.Null);
                Assert.That(strategy.GetType().Name, Is.EqualTo(expectedTypeName));
            }
            finally
            {
                UncertaintyPolicies.DivisionBehavior = prev;
            }
        }

        /// <summary>
        /// Setting a valid tolerance should update the stored value.
        /// </summary>
        [Test]
        public void SetDivisionTolerance_AllowsValidValue()
        {
            double prev = UncertaintyPolicies.DivisionTolerance;
            try
            {
                UncertaintyPolicies.SetDivisionTolerance(1e-307);
                Assert.That(UncertaintyPolicies.DivisionTolerance, Is.EqualTo(1e-307));
            }
            finally
            {
                UncertaintyPolicies.SetDivisionTolerance(prev);
            }
        }

        /// <summary>
        /// Invalid tolerance inputs must trigger guard exceptions.
        /// </summary>
        [Test]
        public void SetDivisionTolerance_Invalid_Throws()
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
