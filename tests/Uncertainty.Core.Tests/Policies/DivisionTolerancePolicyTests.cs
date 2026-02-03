using NUnit.Framework;
using System;
using Uncertainty.Core.Policies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    /// <summary>
    /// Tests for division tolerance policy configuration and guardrails.
    /// </summary>
    public sealed class DivisionTolerancePolicyTests
    {
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

        /// <summary>
        /// SetDivisionTolerance should be settable to 0.0 (reset to default).
        /// </summary>
        [Test]
        public void SetDivisionTolerance_CanResetToZero()
        {
            double prev = UncertaintyPolicies.DivisionTolerance;
            try
            {
                UncertaintyPolicies.SetDivisionTolerance(1e-100);
                Assert.That(UncertaintyPolicies.DivisionTolerance, Is.EqualTo(1e-100));

                UncertaintyPolicies.SetDivisionTolerance(0.0);
                Assert.That(UncertaintyPolicies.DivisionTolerance, Is.EqualTo(0.0));
            }
            finally
            {
                UncertaintyPolicies.SetDivisionTolerance(prev);
            }
        }

        /// <summary>
        /// Verify that DivisionTolerance getter returns the currently set value.
        /// </summary>
        [Test]
        public void DivisionTolerance_Getter_ReturnsCurrentValue()
        {
            double prev = UncertaintyPolicies.DivisionTolerance;
            try
            {
                double testValue = 1.5e-100;
                UncertaintyPolicies.SetDivisionTolerance(testValue);

                double retrieved = UncertaintyPolicies.DivisionTolerance;
                Assert.That(retrieved, Is.EqualTo(testValue));
            }
            finally
            {
                UncertaintyPolicies.SetDivisionTolerance(prev);
            }
        }
    }
}
