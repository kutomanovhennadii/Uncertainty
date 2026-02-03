using NUnit.Framework;
using System.Globalization;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    public sealed class UDoubleArithmeticTests
    {
        /// <summary>
        /// Verifies dividing by an exact-zero UDouble throws DivideByZeroException.
        /// </summary>
        [Test]
        public void Divide_ByZero_Throws()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(0.0, 0.0);

            Assert.That(() => UDouble.Divide(a, b), Throws.TypeOf<DivideByZeroException>());
        }

        /// <summary>
        /// Verifies Divide treats tiny denominators within DivisionTolerance as zero and throws.
        /// </summary>
        [Test]
        public void Divide_TinyDenominator_WithTolerance_Throws()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            double prev = UDouble.DivisionTolerance;
            try
            {
                UDouble.DivisionTolerance = 1e-307;
                Assert.That(() => UDouble.Divide(a, b), Throws.TypeOf<DivideByZeroException>());
            }
            finally
            {
                UDouble.DivisionTolerance = prev;
            }
        }

        /// <summary>
        /// Verifies variance saturation occurs during division when variance would exceed AbsoluteVarianceMax.
        /// </summary>
        [Test]
        public void Divide_LargeVariance_IsSaturated()
        {
            var a = UDouble.FromMeanVar(1.0, 1e308);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            var r = UDouble.Divide(a, b);

            Assert.That(r.Variance, Is.EqualTo(1e300));
        }

        /// <summary>
        /// Verifies addition saturates variance above the configured maximum.
        /// </summary>
        [Test]
        public void Add_Variance_IsSaturated()
        {
            var a = UDouble.FromMeanVar(0.0, 1e301);
            var b = UDouble.FromMeanVar(0.0, 0.0);

            var r = UDouble.Add(a, b);

            Assert.That(r.Variance, Is.EqualTo(1e300));
        }

        /// <summary>
        /// Verifies multiplication saturates variance when result would exceed AbsoluteVarianceMax.
        /// </summary>
        [Test]
        public void Multiply_Variance_IsSaturated()
        {
            var a = UDouble.FromMeanVar(1e152, 1e300);
            var b = UDouble.FromMeanVar(1e152, 0.0);

            var r = UDouble.Multiply(a, b);

            Assert.That(r.Variance, Is.EqualTo(1e300));
        }
    }
}
