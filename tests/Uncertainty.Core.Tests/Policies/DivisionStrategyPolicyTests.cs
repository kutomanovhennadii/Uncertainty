using NUnit.Framework;
using System;
using Uncertainty.Core.Policies;
using DivisionStrategies = Uncertainty.Core.Policies.DivisionPolicies.DivisionStrategies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    /// <summary>
    /// Tests for division strategy policy behaviors and mapping.
    /// </summary>
    public sealed class DivisionStrategyPolicyTests
    {
        /// <summary>
        /// DivisionStrategy setter should reject null.
        /// </summary>
        [Test]
        public void DivisionStrategy_Setter_RejectsNull()
        {
            Assert.That(() => UncertaintyPolicies.DivisionStrategy = null!,
                Throws.TypeOf<ArgumentNullException>());
        }

        /// <summary>
        /// DivisionStrategy getter should lazily initialize when null.
        /// </summary>
        [Test]
        public void DivisionStrategy_Getter_LazilyInitializes()
        {
            var prev = UncertaintyPolicies.DivisionBehavior;
            try
            {
                UncertaintyPolicies.DivisionBehavior = DivisionBehavior.ThrowOnSmallDenominator;
                var strategy = UncertaintyPolicies.DivisionStrategy;

                Assert.That(strategy, Is.Not.Null);
                Assert.That(strategy.GetType().Name, Is.EqualTo("ThrowingDivisionStrategy"));
            }
            finally
            {
                UncertaintyPolicies.DivisionBehavior = prev;
            }
        }

        /// <summary>
        /// DivisionStrategy setter with IMappedDivisionBehavior should update enum.
        /// </summary>
        [Test]
        public void DivisionStrategy_SetterWithMappedBehavior_UpdatesEnum()
        {
            var prev = UncertaintyPolicies.DivisionBehavior;
            try
            {
                var strategy = new DivisionStrategies.SaturatingDivisionStrategy();
                UncertaintyPolicies.DivisionStrategy = strategy;

                var current = UncertaintyPolicies.DivisionBehavior;
                Assert.That(current, Is.EqualTo(DivisionBehavior.SaturateVariance));
            }
            finally
            {
                UncertaintyPolicies.DivisionBehavior = prev;
            }
        }
    }
}
