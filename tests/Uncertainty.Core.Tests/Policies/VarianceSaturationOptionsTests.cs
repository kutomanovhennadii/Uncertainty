using NUnit.Framework;
using System;
using Uncertainty.Core.Policies.VarianceSaturationPolicies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    /// <summary>
    /// Tests for VarianceSaturationOptions validation and default values.
    /// </summary>
    public sealed class VarianceSaturationOptionsTests
    {
        /// <summary>
        /// Default static property should provide valid default values.
        /// </summary>
        [Test]
        public void Default_ProvidesValidValues()
        {
            var opts = VarianceSaturationOptions.Default;

            Assert.That(opts.MaxRelativeStdDev, Is.EqualTo(1e8));
            Assert.That(opts.AbsoluteVarianceMax, Is.EqualTo(1e300));
            Assert.DoesNotThrow(() => opts.EnsureValid());
        }

        /// <summary>
        /// Constructor should accept valid values.
        /// </summary>
        [Test]
        public void Constructor_AcceptsValidValues()
        {
            var opts = new VarianceSaturationOptions(1e9, 1e301);

            Assert.That(opts.MaxRelativeStdDev, Is.EqualTo(1e9));
            Assert.That(opts.AbsoluteVarianceMax, Is.EqualTo(1e301));
            Assert.DoesNotThrow(() => opts.EnsureValid());
        }

        /// <summary>
        /// EnsureValid should throw when MaxRelativeStdDev is less than 1.0.
        /// </summary>
        [Test]
        public void EnsureValid_ThrowsWhen_MaxRelativeStdDev_LessThanOne()
        {
            var opts = new VarianceSaturationOptions(0.5, 1e300);

            Assert.That(() => opts.EnsureValid(),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName").EqualTo("MaxRelativeStdDev"));
        }

        /// <summary>
        /// EnsureValid should throw when MaxRelativeStdDev is NaN.
        /// </summary>
        [Test]
        public void EnsureValid_ThrowsWhen_MaxRelativeStdDev_IsNaN()
        {
            var opts = new VarianceSaturationOptions(double.NaN, 1e300);

            Assert.That(() => opts.EnsureValid(),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName").EqualTo("MaxRelativeStdDev"));
        }

        /// <summary>
        /// EnsureValid should throw when MaxRelativeStdDev is infinite.
        /// </summary>
        [Test]
        public void EnsureValid_ThrowsWhen_MaxRelativeStdDev_IsInfinite()
        {
            var opts = new VarianceSaturationOptions(double.PositiveInfinity, 1e300);

            Assert.That(() => opts.EnsureValid(),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName").EqualTo("MaxRelativeStdDev"));
        }

        /// <summary>
        /// EnsureValid should throw when AbsoluteVarianceMax is below contract minimum.
        /// </summary>
        [Test]
        public void EnsureValid_ThrowsWhen_AbsoluteVarianceMax_BelowMinimum()
        {
            var opts = new VarianceSaturationOptions(1e8, 1e299);

            Assert.That(() => opts.EnsureValid(),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName").EqualTo("AbsoluteVarianceMax"));
        }

        /// <summary>
        /// EnsureValid should throw when AbsoluteVarianceMax is NaN.
        /// </summary>
        [Test]
        public void EnsureValid_ThrowsWhen_AbsoluteVarianceMax_IsNaN()
        {
            var opts = new VarianceSaturationOptions(1e8, double.NaN);

            Assert.That(() => opts.EnsureValid(),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName").EqualTo("AbsoluteVarianceMax"));
        }

        /// <summary>
        /// EnsureValid should throw when AbsoluteVarianceMax is infinite.
        /// </summary>
        [Test]
        public void EnsureValid_ThrowsWhen_AbsoluteVarianceMax_IsInfinite()
        {
            var opts = new VarianceSaturationOptions(1e8, double.PositiveInfinity);

            Assert.That(() => opts.EnsureValid(),
                Throws.TypeOf<ArgumentOutOfRangeException>()
                    .With.Property("ParamName").EqualTo("AbsoluteVarianceMax"));
        }

        /// <summary>
        /// TryValidate should return true for valid options.
        /// </summary>
        [Test]
        public void TryValidate_ReturnsTrueForValidOptions()
        {
            var opts = new VarianceSaturationOptions(1e9, 1e301);

            bool result = opts.TryValidate(out string? error);

            Assert.That(result, Is.True);
            Assert.That(error, Is.Null);
        }

        /// <summary>
        /// TryValidate should return false for invalid MaxRelativeStdDev.
        /// </summary>
        [Test]
        public void TryValidate_ReturnsFalseForInvalidMaxRelativeStdDev()
        {
            var opts = new VarianceSaturationOptions(0.5, 1e300);

            bool result = opts.TryValidate(out string? error);

            Assert.That(result, Is.False);
            Assert.That(error, Is.Not.Null);
            Assert.That(error, Does.Contain("MaxRelativeStdDev"));
        }

        /// <summary>
        /// TryValidate should return false for invalid AbsoluteVarianceMax.
        /// </summary>
        [Test]
        public void TryValidate_ReturnsFalseForInvalidAbsoluteVarianceMax()
        {
            var opts = new VarianceSaturationOptions(1e8, 1e299);

            bool result = opts.TryValidate(out string? error);

            Assert.That(result, Is.False);
            Assert.That(error, Is.Not.Null);
            Assert.That(error, Does.Contain("AbsoluteVarianceMax"));
        }

        /// <summary>
        /// Constants should have expected values.
        /// </summary>
        [Test]
        public void Constants_HaveExpectedValues()
        {
            Assert.That(VarianceSaturationOptions.DefaultMaxRelativeStdDev, Is.EqualTo(1e8));
            Assert.That(VarianceSaturationOptions.DefaultAbsoluteVarianceMax, Is.EqualTo(1e300));
        }
    }
}
