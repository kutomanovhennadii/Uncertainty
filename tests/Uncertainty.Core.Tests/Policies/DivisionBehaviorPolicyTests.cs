using NUnit.Framework;
using System;
using Uncertainty.Core.Policies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    /// <summary>
    /// Tests for division behavior policy mapping and configuration.
    /// </summary>
    public sealed class DivisionBehaviorPolicyTests
    {
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
        /// Verify DivisionBehavior getter returns the current value.
        /// </summary>
        [Test]
        public void DivisionBehavior_Getter_ReturnsCurrentValue()
        {
            var prev = UncertaintyPolicies.DivisionBehavior;
            try
            {
                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.SaturateVariance;
                Assert.That(UncertaintyPolicies.DivisionBehavior, Is.EqualTo(DivisionBehavior.SaturateVariance));

                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ReturnInfinityMean;
                Assert.That(UncertaintyPolicies.DivisionBehavior, Is.EqualTo(DivisionBehavior.ReturnInfinityMean));

                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;
                Assert.That(UncertaintyPolicies.DivisionBehavior, Is.EqualTo(DivisionBehavior.ThrowOnSmallDenominator));
            }
            finally
            {
                UncertaintyPolicies.DivisionBehavior = prev;
            }
        }

        /// <summary>
        /// Setting invalid enum value should throw.
        /// </summary>
        [Test]
        public void DivisionBehavior_InvalidEnumValue_Throws()
        {
            Assert.That(
                () => UncertaintyPolicies.DivisionBehavior = (DivisionBehavior)999,
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// Changing DivisionBehavior multiple times should update strategy correctly.
        /// </summary>
        [Test]
        public void DivisionBehavior_MultipleChanges_UpdatesStrategy()
        {
            var prev = UncertaintyPolicies.DivisionBehavior;
            try
            {
                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;
                Assert.That(UncertaintyPolicies.DivisionStrategy.GetType().Name,
                    Is.EqualTo("ThrowingDivisionStrategy"));

                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.SaturateVariance;
                Assert.That(UncertaintyPolicies.DivisionStrategy.GetType().Name,
                    Is.EqualTo("SaturatingDivisionStrategy"));

                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ReturnInfinityMean;
                Assert.That(UncertaintyPolicies.DivisionStrategy.GetType().Name,
                    Is.EqualTo("ReturnInfinityDivisionStrategy"));

                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;
                Assert.That(UncertaintyPolicies.DivisionStrategy.GetType().Name,
                    Is.EqualTo("ThrowingDivisionStrategy"));
            }
            finally
            {
                UncertaintyPolicies.DivisionBehavior = prev;
            }
        }
    }
}
