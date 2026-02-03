using NUnit.Framework;
using System.Globalization;

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
            Assert.Throws<ArgumentOutOfRangeException>(() =>
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
            Assert.Throws<ArgumentOutOfRangeException>(() =>
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
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void FromDouble_PositiveInfinity_Throws()
        {
            Assert.That(() => UDouble.FromDouble(double.PositiveInfinity),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void FromDouble_NegativeInfinity_Throws()
        {
            Assert.That(() => UDouble.FromDouble(double.NegativeInfinity),
                Throws.TypeOf<ArgumentException>());
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
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void FromFloat_PositiveInfinity_Throws()
        {
            Assert.That(() => UDouble.FromFloat(float.PositiveInfinity),
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void FromFloat_NegativeInfinity_Throws()
        {
            Assert.That(() => UDouble.FromFloat(float.NegativeInfinity),
                Throws.TypeOf<ArgumentException>());
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
                Throws.TypeOf<ArgumentException>());
        }

        [Test]
        public void ExplicitFloatConversion_Infinity_Throws()
        {
            Assert.That(
                () => { var _ = (UDouble)float.PositiveInfinity; },
                Throws.TypeOf<ArgumentException>());
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

        #region // --- New tests: Welford, Subnormals, ToString, Zero ---

        [Test]
        public void FromDouble_Subnormal_UsesHalfUlpSquaredVariance()
        {
            double sub = BitConverter.Int64BitsToDouble(1);
            var x = UDouble.FromDouble(sub);
            var expected = Math.Pow(0.5 * double.Epsilon, 2);
            Assert.That(x.Variance, Is.EqualTo(expected));
        }

        [Test]
        public void FromFloat_Subnormal_UsesHalfUlpSquaredVariance()
        {
            float sub = BitConverter.Int32BitsToSingle(1);
            var x = UDouble.FromFloat(sub);
            var expected = Math.Pow(0.5 * float.Epsilon, 2);
            Assert.That(x.Variance, Is.EqualTo(expected));
        }

        [Test]
        public void FromData_NumericLargeValues_StableWelfordComputation()
        {
            double baseValue = 1e16;
            int n = 1000;
            var data = System.Linq.Enumerable.Range(0, n).Select(i => baseValue + i).ToArray();

            var r = UDouble.FromData(data);

            // compute expected using decimal for higher precision
            decimal meanD = 0m;
            for (int i = 0; i < n; i++)
                meanD += (decimal)data[i];
            meanD /= n;

            decimal sum = 0m;
            for (int i = 0; i < n; i++)
            {
                decimal delta = (decimal)data[i] - meanD;
                sum += delta * delta;
            }

            decimal sigma2Stat = n > 1 ? sum / (n - 1) : 0m;
            decimal varianceStat = sigma2Stat / n;

            double expectedMean = (double)meanD;
            double expectedVariance = (double)varianceStat;

            Assert.That(r.Mean, Is.EqualTo(expectedMean).Within(1e-6));
            Assert.That(r.Variance, Is.EqualTo(expectedVariance).Within(Math.Max(1e-12, Math.Abs(expectedVariance) * 1e-12)));
        }

        [Test]
        public void FromData_UDoubleLargeValues_StableWelfordComputation()
        {
            double baseValue = 1e16;
            int n = 1000;
            var items = System.Linq.Enumerable.Range(0, n).Select(i => UDouble.FromMeanVar(baseValue + i, 0.0)).ToArray();

            var r = UDouble.FromData(items);

            decimal meanD = 0m;
            for (int i = 0; i < n; i++)
                meanD += (decimal)(baseValue + i);
            meanD /= n;

            decimal sum = 0m;
            for (int i = 0; i < n; i++)
            {
                decimal delta = (decimal)(baseValue + i) - meanD;
                sum += delta * delta;
            }

            decimal sigma2Stat = n > 1 ? sum / (n - 1) : 0m;
            decimal varianceStat = sigma2Stat / n;

            double expectedMean = (double)meanD;
            double expectedVariance = (double)varianceStat;

            Assert.That(r.Mean, Is.EqualTo(expectedMean).Within(1e-6));
            Assert.That(r.Variance, Is.EqualTo(expectedVariance).Within(Math.Max(1e-12, Math.Abs(expectedVariance) * 1e-12)));
        }

        [Test]
        public void ToString_DefaultAndFormat_ReturnsExpected()
        {
            var u = UDouble.FromMeanVar(1.2345, 0.25);

            var defaultStr = u.ToString(null, System.Globalization.CultureInfo.InvariantCulture);
            Assert.That(defaultStr, Is.EqualTo("1.2345 ± 0.5"));

            var f2 = u.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
            Assert.That(f2, Is.EqualTo("1.23 ± 0.50"));
        }

        [Test]
        public void ToString_UsesCurrentCulture()
        {
            var u = UDouble.FromMeanVar(1.2345, 0.25);

            var prev = CultureInfo.CurrentCulture;
            try
            {
                CultureInfo.CurrentCulture = new CultureInfo("fr-FR");
                var s = u.ToString();
                Assert.That(s, Is.EqualTo("1,2345 ± 0,5"));
            }
            finally
            {
                CultureInfo.CurrentCulture = prev;
            }
        }

        [Test]
        public void Zero_IsZeroAndExact()
        {
            Assert.That(UDouble.Zero.Mean, Is.EqualTo(0.0));
            Assert.That(UDouble.Zero.Variance, Is.EqualTo(0.0));
        }

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

        #endregion

    }
}
