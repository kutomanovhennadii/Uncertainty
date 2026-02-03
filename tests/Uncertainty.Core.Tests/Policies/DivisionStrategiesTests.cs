using NUnit.Framework;
using System;
using Uncertainty.Core;
using Uncertainty.Core.Policies;
using Uncertainty.Core.Policies.DivisionPolicies.DivisionStrategies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    /// <summary>
    /// Unit tests for internal division strategy implementations.
    /// These tests verify that strategies correctly implement their mapped behaviors
    /// and handle edge cases appropriately.
    /// </summary>
    public sealed class DivisionStrategiesTests
    {
        /// <summary>
        /// ThrowingDivisionStrategy should throw DivideByZeroException.
        /// </summary>
        [Test]
        public void ThrowingStrategy_ThrowsException()
        {
            var strategy = new ThrowingDivisionStrategy();
            var a = UDouble.FromMeanVar(10, 1);
            var b = UDouble.FromMeanVar(1e-308, 0.5);

            Assert.That(() => strategy.HandleSmallDenominator(a, b),
                Throws.TypeOf<DivideByZeroException>());
        }

        /// <summary>
        /// ThrowingDivisionStrategy should map to ThrowOnSmallDenominator behavior.
        /// </summary>
        [Test]
        public void ThrowingStrategy_MapsToCorrectBehavior()
        {
            var strategy = new ThrowingDivisionStrategy();
            var mapper = strategy as IMappedDivisionBehavior;

            Assert.That(mapper, Is.Not.Null);
            Assert.That(mapper!.MappedBehavior, Is.EqualTo(DivisionBehavior.ThrowOnSmallDenominator));
        }

        /// <summary>
        /// SaturatingDivisionStrategy should return finite result with saturated variance.
        /// </summary>
        [Test]
        public void SaturatingStrategy_ReturnsSaturatedResult()
        {
            var strategy = new SaturatingDivisionStrategy();
            var a = UDouble.FromMeanVar(10, 100);
            var b = UDouble.FromMeanVar(0.01, 0.001);

            var result = strategy.HandleSmallDenominator(a, b);

            Assert.That(double.IsFinite(result.Mean), Is.True);
            Assert.That(double.IsFinite(result.Variance), Is.True);
            Assert.That(result.Variance, Is.GreaterThan(0));
        }

        /// <summary>
        /// SaturatingDivisionStrategy should map to SaturateVariance behavior.
        /// </summary>
        [Test]
        public void SaturatingStrategy_MapsToCorrectBehavior()
        {
            var strategy = new SaturatingDivisionStrategy();
            var mapper = strategy as IMappedDivisionBehavior;

            Assert.That(mapper, Is.Not.Null);
            Assert.That(mapper!.MappedBehavior, Is.EqualTo(DivisionBehavior.SaturateVariance));
        }

        /// <summary>
        /// ReturnInfinityDivisionStrategy should return finite result (current implementation).
        /// </summary>
        [Test]
        public void ReturnInfinityStrategy_ReturnsFiniteResult()
        {
            var strategy = new ReturnInfinityDivisionStrategy();
            var a = UDouble.FromMeanVar(10, 100);
            var b = UDouble.FromMeanVar(0.01, 0.001);

            var result = strategy.HandleSmallDenominator(a, b);

            Assert.That(double.IsFinite(result.Mean), Is.True);
            Assert.That(double.IsFinite(result.Variance), Is.True);
        }

        /// <summary>
        /// ReturnInfinityDivisionStrategy should map to ReturnInfinityMean behavior.
        /// </summary>
        [Test]
        public void ReturnInfinityStrategy_MapsToCorrectBehavior()
        {
            var strategy = new ReturnInfinityDivisionStrategy();
            var mapper = strategy as IMappedDivisionBehavior;

            Assert.That(mapper, Is.Not.Null);
            Assert.That(mapper!.MappedBehavior, Is.EqualTo(DivisionBehavior.ReturnInfinityMean));
        }
    }
}
