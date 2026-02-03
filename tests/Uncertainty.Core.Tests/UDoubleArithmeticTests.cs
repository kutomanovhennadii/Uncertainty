using NUnit.Framework;
using System.Globalization;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    public sealed class UDoubleArithmeticTests
    {
        [Test]
        public void Divide_ByZero_Throws()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(0.0, 0.0);

            Assert.That(() => UDouble.Divide(a, b), Throws.TypeOf<DivideByZeroException>());
        }

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

        [Test]
        public void Divide_LargeVariance_IsSaturated()
        {
            var a = UDouble.FromMeanVar(1.0, 1e308);
            var b = UDouble.FromMeanVar(1e-308, 0.0);

            var r = UDouble.Divide(a, b);

            Assert.That(r.Variance, Is.EqualTo(1e300));
        }

        [Test]
        public void Add_Variance_IsSaturated()
        {
            var a = UDouble.FromMeanVar(0.0, 1e301);
            var b = UDouble.FromMeanVar(0.0, 0.0);

            var r = UDouble.Add(a, b);

            Assert.That(r.Variance, Is.EqualTo(1e300));
        }

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
