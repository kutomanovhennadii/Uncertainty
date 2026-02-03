using NUnit.Framework;
using Uncertainty.Core.Policies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    /// <summary>
    /// Validates policy defaults and initial state.
    /// </summary>
    public sealed class UncertaintyPoliciesDefaultsTests
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

            var varOpts = UncertaintyPolicies.VarianceSaturation;
            Assert.That(varOpts.MaxRelativeStdDev, Is.EqualTo(1e8));
            Assert.That(varOpts.AbsoluteVarianceMax, Is.EqualTo(1e300));
        }
    }
}
