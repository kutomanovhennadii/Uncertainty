using NUnit.Framework;
using System;
using Uncertainty.Core.Policies;
using Uncertainty.Core.Policies.VarianceSaturationPolicies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    /// <summary>
    /// Tests for variance saturation configuration on the policy facade.
    /// </summary>
    public sealed class VarianceSaturationPolicyFacadeTests
    {
        /// <summary>
        /// VarianceSaturation getter should return current options.
        /// </summary>
        [Test]
        public void VarianceSaturation_ReturnsCurrentOptions()
        {
            var prev = UncertaintyPolicies.VarianceSaturation;
            try
            {
                var newOpts = new VarianceSaturationOptions(1e9, 1e301);
                UncertaintyPolicies.ConfigureVarianceSaturation(newOpts);

                var current = UncertaintyPolicies.VarianceSaturation;
                Assert.That(current.MaxRelativeStdDev, Is.EqualTo(1e9));
                Assert.That(current.AbsoluteVarianceMax, Is.EqualTo(1e301));
            }
            finally
            {
                UncertaintyPolicies.ConfigureVarianceSaturation(prev);
            }
        }

        /// <summary>
        /// ConfigureVarianceSaturation should accept valid options.
        /// </summary>
        [Test]
        public void ConfigureVarianceSaturation_AcceptsValidOptions()
        {
            var prev = UncertaintyPolicies.VarianceSaturation;
            try
            {
                var newOpts = new VarianceSaturationOptions(1e9, 1e301);
                Assert.DoesNotThrow(() => UncertaintyPolicies.ConfigureVarianceSaturation(newOpts));

                var current = UncertaintyPolicies.VarianceSaturation;
                Assert.That(current.MaxRelativeStdDev, Is.EqualTo(1e9));
                Assert.That(current.AbsoluteVarianceMax, Is.EqualTo(1e301));
            }
            finally
            {
                UncertaintyPolicies.ConfigureVarianceSaturation(prev);
            }
        }

        /// <summary>
        /// ConfigureVarianceSaturation should reject default(struct).
        /// </summary>
        [Test]
        public void ConfigureVarianceSaturation_RejectsDefaultStruct()
        {
            Assert.That(() => UncertaintyPolicies.ConfigureVarianceSaturation(default),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// ConfigureVarianceSaturation with default struct should throw.
        /// </summary>
        [Test]
        public void ConfigureVarianceSaturation_WithDefaultStruct_ThrowsArgumentException()
        {
            var prev = UncertaintyPolicies.VarianceSaturation;
            try
            {
                Assert.That(
                    () => UncertaintyPolicies.ConfigureVarianceSaturation(default(VarianceSaturationOptions)),
                    Throws.TypeOf<ArgumentException>().With.Property("ParamName").EqualTo("options"));
            }
            finally
            {
                UncertaintyPolicies.ConfigureVarianceSaturation(prev);
            }
        }

        /// <summary>
        /// ConfigureVarianceSaturation should validate constraints.
        /// </summary>
        [Test]
        public void ConfigureVarianceSaturation_ValidatesConstraints()
        {
            var badOpts1 = new VarianceSaturationOptions(0.5, 1e300);
            Assert.That(() => UncertaintyPolicies.ConfigureVarianceSaturation(badOpts1),
                Throws.TypeOf<ArgumentOutOfRangeException>());

            var badOpts2 = new VarianceSaturationOptions(1e8, 1e299);
            Assert.That(() => UncertaintyPolicies.ConfigureVarianceSaturation(badOpts2),
                Throws.TypeOf<ArgumentOutOfRangeException>());

            var badOpts3 = new VarianceSaturationOptions(double.NaN, 1e300);
            Assert.That(() => UncertaintyPolicies.ConfigureVarianceSaturation(badOpts3),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        /// <summary>
        /// ConfigureVarianceSaturation should reject non-default invalid options and throw.
        /// </summary>
        [Test]
        public void ConfigureVarianceSaturation_RejectsInvalidNonDefault()
        {
            var prev = UncertaintyPolicies.VarianceSaturation;
            try
            {
                Assert.That(
                    () => UncertaintyPolicies.ConfigureVarianceSaturation(
                        new VarianceSaturationOptions(-1e8, 1e300)),
                    Throws.TypeOf<ArgumentOutOfRangeException>());
            }
            finally
            {
                UncertaintyPolicies.ConfigureVarianceSaturation(prev);
            }
        }

        /// <summary>
        /// VarianceSaturation should reflect changes from ConfigureVarianceSaturation across multiple calls.
        /// </summary>
        [Test]
        public void VarianceSaturation_ReflectsMultipleConfigurationChanges()
        {
            var prev = UncertaintyPolicies.VarianceSaturation;
            try
            {
                var opts1 = new VarianceSaturationOptions(1e9, 1e301);
                UncertaintyPolicies.ConfigureVarianceSaturation(opts1);
                var current1 = UncertaintyPolicies.VarianceSaturation;
                Assert.That(current1.MaxRelativeStdDev, Is.EqualTo(1e9));
                Assert.That(current1.AbsoluteVarianceMax, Is.EqualTo(1e301));

                var opts2 = new VarianceSaturationOptions(2e8, 1e302);
                UncertaintyPolicies.ConfigureVarianceSaturation(opts2);
                var current2 = UncertaintyPolicies.VarianceSaturation;
                Assert.That(current2.MaxRelativeStdDev, Is.EqualTo(2e8));
                Assert.That(current2.AbsoluteVarianceMax, Is.EqualTo(1e302));

                var opts3 = VarianceSaturationOptions.Default;
                UncertaintyPolicies.ConfigureVarianceSaturation(opts3);
                var current3 = UncertaintyPolicies.VarianceSaturation;
                Assert.That(current3.MaxRelativeStdDev, Is.EqualTo(1e8));
                Assert.That(current3.AbsoluteVarianceMax, Is.EqualTo(1e300));
            }
            finally
            {
                UncertaintyPolicies.ConfigureVarianceSaturation(prev);
            }
        }
    }
}
