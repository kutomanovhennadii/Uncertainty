using NUnit.Framework;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    public sealed class UDoubleFactoryTests
    {
        #region // ---------- FromMeanVar Tests ----------

        [Test]
        public void FromMeanVar_ValidInputs_ReturnsExpectedUDouble()
        {
            var x = UDouble.FromMeanVar(10.0, 4.0);

            Assert.That(x.Mean, Is.EqualTo(10.0));
            Assert.That(x.Variance, Is.EqualTo(4.0));
            Assert.That(x.StdDev, Is.EqualTo(2.0));
        }

        [Test]
        public void FromMeanVar_ZeroVariance_CreatesExactValue()
        {
            var x = UDouble.FromMeanVar(5.0, 0.0);

            Assert.That(x.Mean, Is.EqualTo(5.0));
            Assert.That(x.Variance, Is.EqualTo(0.0));
            Assert.That(x.StdDev, Is.EqualTo(0.0));
        }

        [Test]
        public void FromMeanVar_NegativeVariance_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UDouble.FromMeanVar(5.0, -0.1);
            });
        }

        [Test]
        public void FromMeanVar_NaNMean_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UDouble.FromMeanVar(double.NaN, 1.0);
            });
        }

        [Test]
        public void FromMeanVar_InfiniteMean_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UDouble.FromMeanVar(double.PositiveInfinity, 1.0);
            });
        }

        [Test]
        public void FromMeanVar_NaNVariance_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UDouble.FromMeanVar(0.0, double.NaN);
            });
        }

        [Test]
        public void FromMeanVar_InfiniteVariance_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UDouble.FromMeanVar(0.0, double.PositiveInfinity);
            });
        }
        #endregion

        #region // ---------- FromMeanStd Tests ----------

        [Test]
        public void FromMeanStd_ValidInputs_ReturnsUDouble()
        {
            var x = UDouble.FromMeanStd(mean: 5.0, stdDev: 2.0);

            Assert.That(x.Mean, Is.EqualTo(5.0));
            Assert.That(x.Variance, Is.EqualTo(4.0));
            Assert.That(x.StdDev, Is.EqualTo(2.0));
        }

        [Test]
        public void FromMeanStd_ZeroStdDev_ReturnsExactNumber()
        {
            var x = UDouble.FromMeanStd(mean: 3.0, stdDev: 0.0);

            Assert.That(x.Mean, Is.EqualTo(3.0));
            Assert.That(x.Variance, Is.EqualTo(0.0));
            Assert.That(x.StdDev, Is.EqualTo(0.0));
        }


        [Test]
        public void FromMeanStd_NegativeStdDev_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(1.0, -0.1));
        }

        [Test]
        public void FromMeanStd_NaNMean_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(double.NaN, 1.0));
        }

        [Test]
        public void FromMeanStd_InfiniteMean_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(double.PositiveInfinity, 1.0));
        }

        [Test]
        public void FromMeanStd_NaNStdDev_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(1.0, double.NaN));
        }

        [Test]
        public void FromMeanStd_InfiniteStdDev_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(1.0, double.PositiveInfinity));
        }
        #endregion

        #region // --- FromDouble ---

        [Test]
        public void FromDouble_FiniteValue_ReturnsMeanAndPositiveVariance()
        {
            var x = UDouble.FromDouble(1.0);

            Assert.That(x.Mean, Is.EqualTo(1.0));
            Assert.That(x.Variance, Is.GreaterThan(0.0));
            Assert.That(x.StdDev, Is.GreaterThan(0.0));
        }

        [Test]
        public void FromDouble_Zero_ReturnsMeanZeroAndNonNegativeFiniteVariance()
        {
            var x = UDouble.FromDouble(0.0);

            Assert.That(x.Mean, Is.EqualTo(0.0));
            Assert.That(x.Variance, Is.GreaterThanOrEqualTo(0.0));
            Assert.That(double.IsFinite(x.Variance), Is.True);
        }

        [Test]
        public void FromDouble_NaN_Throws()
        {
            Assert.That(() => UDouble.FromDouble(double.NaN),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void FromDouble_PositiveInfinity_Throws()
        {
            Assert.That(() => UDouble.FromDouble(double.PositiveInfinity),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void FromDouble_NegativeInfinity_Throws()
        {
            Assert.That(() => UDouble.FromDouble(double.NegativeInfinity),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void FromDouble_KnownValue_UsesHalfUlpSquaredVariance()
        {
            var x = UDouble.FromDouble(1.0);

            // ulp(1.0) = 2^-52 for IEEE-754 double
            var ulp = Math.Pow(2.0, -52);
            var expected = Math.Pow(0.5 * ulp, 2);

            Assert.That(x.Variance, Is.EqualTo(expected));
        }
        #endregion

        #region // --- FromFloat ---

        [Test]
        public void FromFloat_FiniteValue_ReturnsMeanAndPositiveVariance()
        {
            var x = UDouble.FromFloat(1.0f);

            Assert.That(x.Mean, Is.EqualTo(1.0));
            Assert.That(x.Variance, Is.GreaterThan(0.0));
            Assert.That(x.StdDev, Is.GreaterThan(0.0));
        }

        [Test]
        public void FromFloat_Zero_ReturnsMeanZeroAndNonNegativeFiniteVariance()
        {
            var x = UDouble.FromFloat(0.0f);

            Assert.That(x.Mean, Is.EqualTo(0.0));
            Assert.That(x.Variance, Is.GreaterThanOrEqualTo(0.0));
            Assert.That(double.IsFinite(x.Variance), Is.True);
        }

        [Test]
        public void FromFloat_NaN_Throws()
        {
            Assert.That(() => UDouble.FromFloat(float.NaN),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void FromFloat_PositiveInfinity_Throws()
        {
            Assert.That(() => UDouble.FromFloat(float.PositiveInfinity),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void FromFloat_NegativeInfinity_Throws()
        {
            Assert.That(() => UDouble.FromFloat(float.NegativeInfinity),
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void FromFloat_KnownValue_UsesHalfUlpSquaredVariance()
        {
            var x = UDouble.FromFloat(1.0f);

            // ulp(1.0f) = 2^-23 for IEEE-754 float
            var ulp = Math.Pow(2.0, -23);
            var expected = Math.Pow(0.5 * ulp, 2);

            Assert.That(x.Variance, Is.EqualTo(expected));
        }
        #endregion

        #region // --- Explicit conversions ---

        [Test]
        public void ExplicitDoubleConversion_UsesFromDouble()
        {
            double value = 1.5;
            var viaOperator = (UDouble)value;
            var viaFactory = UDouble.FromDouble(value);

            Assert.That(viaOperator, Is.EqualTo(viaFactory));
        }

        [Test]
        public void ExplicitFloatConversion_UsesFromFloat()
        {
            float value = 2.5f;
            var viaOperator = (UDouble)value;
            var viaFactory = UDouble.FromFloat(value);

            Assert.That(viaOperator, Is.EqualTo(viaFactory));
        }

        [Test]
        public void ExplicitIntConversion_UsesFromDouble()
        {
            int value = 7;
            var viaOperator = (UDouble)value;
            var viaFactory = UDouble.FromDouble((double)value);

            Assert.That(viaOperator, Is.EqualTo(viaFactory));
        }

        [Test]
        public void ExplicitDoubleConversion_NaN_Throws()
        {
            Assert.That(
                () => { var _ = (UDouble)double.NaN; },
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }

        [Test]
        public void ExplicitFloatConversion_Infinity_Throws()
        {
            Assert.That(
                () => { var _ = (UDouble)float.PositiveInfinity; },
                Throws.TypeOf<ArgumentOutOfRangeException>());
        }
        #endregion

        #region // --- FromData(IEnumerable<UDouble>) ---

        [Test]
        public void FromData_UDoubleSingleElement_ReturnsSameMeanAndVariance()
        {
            var x = UDouble.FromMeanVar(5.0, 2.0);

            var r = UDouble.FromData(new[] { x });

            Assert.That(r.Mean, Is.EqualTo(5.0));
            Assert.That(r.Variance, Is.EqualTo(2.0));
        }

        [Test]
        public void FromData_UDoubleMultipleElements_ComputesStatAndInstrumentalVariance()
        {
            var a = UDouble.FromMeanVar(1.0, 1.0);
            var b = UDouble.FromMeanVar(3.0, 1.0);

            var r = UDouble.FromData(new[] { a, b });

            // mean = (1 + 3) / 2 = 2
            // sigma^2_stat = ((1-2)^2 + (3-2)^2) / (2-1) = 2
            // variance_stat = 2 / 2 = 1
            // variance_inst = (1 + 1) / 2 = 1
            // total = 2
            Assert.That(r.Mean, Is.EqualTo(2.0));
            Assert.That(r.Variance, Is.EqualTo(2.0));
        }

        [Test]
        public void FromData_UDoubleEmpty_Throws()
        {
            Assert.That(
                () => UDouble.FromData(Array.Empty<UDouble>()),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void FromData_UDoubleNull_Throws()
        {
            Assert.That(
                () => UDouble.FromData((IEnumerable<UDouble>)null!),
                Throws.TypeOf<ArgumentNullException>());
        }
        #endregion

        #region // --- FromData(IEnumerable<T>) where T : INumber<T> ---

        [Test]
        public void FromData_NumericInts_ComputesMeanAndNonNegativeVariance()
        {
            int[] data = new[] { 1, 2, 3 };
            var r = UDouble.FromData(data);

            Assert.That(r.Mean, Is.EqualTo(2.0));
            Assert.That(r.Variance, Is.GreaterThanOrEqualTo(0.0));
        }

        [Test]
        public void FromData_NumericFloats_UsesFloatRounding()
        {
            float[] data = new[] { 1.0f, 2.0f, 3.0f };
            var r = UDouble.FromData(data);

            Assert.That(r.Mean, Is.EqualTo(2.0));
            Assert.That(r.Variance, Is.GreaterThan(0.0));
        }

        [Test]
        public void FromData_NumericDoubles_UsesDoubleRounding()
        {
            double[] data = new[] { 1.0, 2.0, 3.0 };
            var r = UDouble.FromData(data);

            Assert.That(r.Mean, Is.EqualTo(2.0));
            Assert.That(r.Variance, Is.GreaterThan(0.0));
        }

        [Test]
        public void FromData_NumericSingleElement_ReturnsInstrumentalVarianceOnly()
        {
            double[] data = new[] { 5.0 };
            var r = UDouble.FromData(data);

            Assert.That(r.Mean, Is.EqualTo(5.0));
            Assert.That(r.Variance, Is.GreaterThanOrEqualTo(0.0));
        }

        [Test]
        public void FromData_NumericEmpty_Throws()
        {
            Assert.That(
                () => UDouble.FromData(Array.Empty<int>()),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void FromData_NumericNull_Throws()
        {
            Assert.That(
                () => UDouble.FromData<int>(null!),
                Throws.TypeOf<ArgumentNullException>());
        }
        #endregion

    }
}
