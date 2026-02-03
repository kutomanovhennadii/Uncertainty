using NUnit.Framework;
using System.Globalization;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    public sealed class UDoubleFactoryTests
    {
        #region // ---------- FromMeanVar Tests ----------

        /// <summary>
        /// Verifies that FromMeanVar returns expected mean and variance for valid inputs.
        /// </summary>
        [Test]
        public void FromMeanVar_ValidInputs_ReturnsExpectedUDouble()
        {
            var x = UDouble.FromMeanVar(10.0, 4.0);

            Assert.That(x.Mean, Is.EqualTo(10.0));
            Assert.That(x.Variance, Is.EqualTo(4.0));
            Assert.That(x.StdDev, Is.EqualTo(2.0));
        }

        /// <summary>
        /// Ensures zero variance creates an exact (no-uncertainty) UDouble value.
        /// </summary>
        [Test]
        public void FromMeanVar_ZeroVariance_CreatesExactValue()
        {
            var x = UDouble.FromMeanVar(5.0, 0.0);

            Assert.That(x.Mean, Is.EqualTo(5.0));
            Assert.That(x.Variance, Is.EqualTo(0.0));
            Assert.That(x.StdDev, Is.EqualTo(0.0));
        }

        /// <summary>
        /// Verifies that passing a negative variance to FromMeanVar throws ArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void FromMeanVar_NegativeVariance_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
            {
                UDouble.FromMeanVar(5.0, -0.1);
            });
        }

        /// <summary>
        /// Verifies that passing NaN as mean to FromMeanVar throws ArgumentException.
        /// </summary>
        [Test]
        public void FromMeanVar_NaNMean_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UDouble.FromMeanVar(double.NaN, 1.0);
            });
        }

        /// <summary>
        /// Verifies that passing an infinite mean to FromMeanVar throws ArgumentException.
        /// </summary>
        [Test]
        public void FromMeanVar_InfiniteMean_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UDouble.FromMeanVar(double.PositiveInfinity, 1.0);
            });
        }

        /// <summary>
        /// Verifies that passing NaN as variance to FromMeanVar throws ArgumentException.
        /// </summary>
        [Test]
        public void FromMeanVar_NaNVariance_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
            {
                UDouble.FromMeanVar(0.0, double.NaN);
            });
        }

        /// <summary>
        /// Verifies that passing infinite variance to FromMeanVar throws ArgumentException.
        /// </summary>
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

        /// <summary>
        /// Verifies that FromMeanStd converts stdDev to variance and returns correct values for valid inputs.
        /// </summary>
        [Test]
        public void FromMeanStd_ValidInputs_ReturnsUDouble()
        {
            var x = UDouble.FromMeanStd(mean: 5.0, stdDev: 2.0);

            Assert.That(x.Mean, Is.EqualTo(5.0));
            Assert.That(x.Variance, Is.EqualTo(4.0));
            Assert.That(x.StdDev, Is.EqualTo(2.0));
        }

        /// <summary>
        /// Ensures FromMeanStd with zero stdDev produces an exact value with zero variance.
        /// </summary>
        [Test]
        public void FromMeanStd_ZeroStdDev_ReturnsExactNumber()
        {
            var x = UDouble.FromMeanStd(mean: 3.0, stdDev: 0.0);

            Assert.That(x.Mean, Is.EqualTo(3.0));
            Assert.That(x.Variance, Is.EqualTo(0.0));
            Assert.That(x.StdDev, Is.EqualTo(0.0));
        }


        /// <summary>
        /// Verifies that negative standard deviation passed to FromMeanStd throws ArgumentOutOfRangeException.
        /// </summary>
        [Test]
        public void FromMeanStd_NegativeStdDev_Throws()
        {
            Assert.Throws<ArgumentOutOfRangeException>(() =>
                UDouble.FromMeanStd(1.0, -0.1));
        }

        /// <summary>
        /// Verifies that passing NaN as mean to FromMeanStd throws ArgumentException.
        /// </summary>
        [Test]
        public void FromMeanStd_NaNMean_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(double.NaN, 1.0));
        }

        /// <summary>
        /// Verifies that passing an infinite mean to FromMeanStd throws ArgumentException.
        /// </summary>
        [Test]
        public void FromMeanStd_InfiniteMean_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(double.PositiveInfinity, 1.0));
        }

        /// <summary>
        /// Verifies that passing NaN as stdDev to FromMeanStd throws ArgumentException.
        /// </summary>
        [Test]
        public void FromMeanStd_NaNStdDev_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(1.0, double.NaN));
        }

        /// <summary>
        /// Verifies that passing infinite stdDev to FromMeanStd throws ArgumentException.
        /// </summary>
        [Test]
        public void FromMeanStd_InfiniteStdDev_Throws()
        {
            Assert.Throws<ArgumentException>(() =>
                UDouble.FromMeanStd(1.0, double.PositiveInfinity));
        }
        #endregion

        #region // --- FromDouble ---

        /// <summary>
        /// Verifies that FromDouble returns a UDouble with the given mean and positive variance for a finite input.
        /// </summary>
        [Test]
        public void FromDouble_FiniteValue_ReturnsMeanAndPositiveVariance()
        {
            var x = UDouble.FromDouble(1.0);

            Assert.That(x.Mean, Is.EqualTo(1.0));
            Assert.That(x.Variance, Is.GreaterThan(0.0));
            Assert.That(x.StdDev, Is.GreaterThan(0.0));
        }

        /// <summary>
        /// Verifies that FromDouble(0.0) returns mean 0 and a non-negative finite variance.
        /// </summary>
        [Test]
        public void FromDouble_Zero_ReturnsMeanZeroAndNonNegativeFiniteVariance()
        {
            var x = UDouble.FromDouble(0.0);

            Assert.That(x.Mean, Is.EqualTo(0.0));
            Assert.That(x.Variance, Is.GreaterThanOrEqualTo(0.0));
            Assert.That(double.IsFinite(x.Variance), Is.True);
        }

        /// <summary>
        /// Verifies that FromDouble throws ArgumentException when passed NaN.
        /// </summary>
        [Test]
        public void FromDouble_NaN_Throws()
        {
            Assert.That(() => UDouble.FromDouble(double.NaN),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Verifies that FromDouble throws ArgumentException when passed positive infinity.
        /// </summary>
        [Test]
        public void FromDouble_PositiveInfinity_Throws()
        {
            Assert.That(() => UDouble.FromDouble(double.PositiveInfinity),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Verifies that FromDouble throws ArgumentException when passed negative infinity.
        /// </summary>
        [Test]
        public void FromDouble_NegativeInfinity_Throws()
        {
            Assert.That(() => UDouble.FromDouble(double.NegativeInfinity),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Confirms that FromDouble computes variance as (0.5*ulp)^2 for a known value.
        /// </summary>
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

        /// <summary>
        /// Verifies FromFloat produces correct mean and positive variance for a finite input.
        /// </summary>
        [Test]
        public void FromFloat_FiniteValue_ReturnsMeanAndPositiveVariance()
        {
            var x = UDouble.FromFloat(1.0f);

            Assert.That(x.Mean, Is.EqualTo(1.0));
            Assert.That(x.Variance, Is.GreaterThan(0.0));
            Assert.That(x.StdDev, Is.GreaterThan(0.0));
        }

        /// <summary>
        /// Verifies FromFloat(0.0f) returns mean 0 and non-negative finite variance.
        /// </summary>
        [Test]
        public void FromFloat_Zero_ReturnsMeanZeroAndNonNegativeFiniteVariance()
        {
            var x = UDouble.FromFloat(0.0f);

            Assert.That(x.Mean, Is.EqualTo(0.0));
            Assert.That(x.Variance, Is.GreaterThanOrEqualTo(0.0));
            Assert.That(double.IsFinite(x.Variance), Is.True);
        }

        /// <summary>
        /// Verifies FromFloat throws ArgumentException when passed NaN.
        /// </summary>
        [Test]
        public void FromFloat_NaN_Throws()
        {
            Assert.That(() => UDouble.FromFloat(float.NaN),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Verifies FromFloat throws ArgumentException when passed positive infinity.
        /// </summary>
        [Test]
        public void FromFloat_PositiveInfinity_Throws()
        {
            Assert.That(() => UDouble.FromFloat(float.PositiveInfinity),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Verifies FromFloat throws ArgumentException when passed negative infinity.
        /// </summary>
        [Test]
        public void FromFloat_NegativeInfinity_Throws()
        {
            Assert.That(() => UDouble.FromFloat(float.NegativeInfinity),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Confirms FromFloat computes variance as (0.5*ulp)^2 for a known float value.
        /// </summary>
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

        /// <summary>
        /// Ensures explicit conversion from double delegates to FromDouble factory.
        /// </summary>
        [Test]
        public void ExplicitDoubleConversion_UsesFromDouble()
        {
            double value = 1.5;
            var viaOperator = (UDouble)value;
            var viaFactory = UDouble.FromDouble(value);

            Assert.That(viaOperator, Is.EqualTo(viaFactory));
        }

        /// <summary>
        /// Ensures explicit conversion from float delegates to FromFloat factory.
        /// </summary>
        [Test]
        public void ExplicitFloatConversion_UsesFromFloat()
        {
            float value = 2.5f;
            var viaOperator = (UDouble)value;
            var viaFactory = UDouble.FromFloat(value);

            Assert.That(viaOperator, Is.EqualTo(viaFactory));
        }

        /// <summary>
        /// Ensures explicit conversion from int delegates to FromDouble factory.
        /// </summary>
        [Test]
        public void ExplicitIntConversion_UsesFromDouble()
        {
            int value = 7;
            var viaOperator = (UDouble)value;
            var viaFactory = UDouble.FromDouble((double)value);

            Assert.That(viaOperator, Is.EqualTo(viaFactory));
        }

        /// <summary>
        /// Verifies explicit conversion from double throws when input is NaN.
        /// </summary>
        [Test]
        public void ExplicitDoubleConversion_NaN_Throws()
        {
            Assert.That(
                () => { var _ = (UDouble)double.NaN; },
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Verifies explicit conversion from float throws when input is infinite.
        /// </summary>
        [Test]
        public void ExplicitFloatConversion_Infinity_Throws()
        {
            Assert.That(
                () => { var _ = (UDouble)float.PositiveInfinity; },
                Throws.TypeOf<ArgumentException>());
        }
        #endregion

        #region // --- FromData(IEnumerable<UDouble>) ---

        /// <summary>
        /// Verifies FromData over UDouble with a single element returns the same mean and variance.
        /// </summary>
        [Test]
        public void FromData_UDoubleSingleElement_ReturnsSameMeanAndVariance()
        {
            var x = UDouble.FromMeanVar(5.0, 2.0);

            var r = UDouble.FromData(new[] { x });

            Assert.That(r.Mean, Is.EqualTo(5.0));
            Assert.That(r.Variance, Is.EqualTo(2.0));
        }

        /// <summary>
        /// Verifies FromData combines statistical and instrumental variance correctly for multiple UDouble inputs.
        /// </summary>
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

        /// <summary>
        /// Verifies FromData throws ArgumentException when given an empty UDouble sequence.
        /// </summary>
        [Test]
        public void FromData_UDoubleEmpty_Throws()
        {
            Assert.That(
                () => UDouble.FromData(Array.Empty<UDouble>()),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Verifies FromData throws ArgumentNullException when passed null UDouble sequence.
        /// </summary>
        [Test]
        public void FromData_UDoubleNull_Throws()
        {
            Assert.That(
                () => UDouble.FromData((IEnumerable<UDouble>)null!),
                Throws.TypeOf<ArgumentNullException>());
        }
        #endregion

        #region // --- FromData(IEnumerable<T>) where T : INumber<T> ---

        /// <summary>
        /// Verifies FromData over integer sequence computes mean and non-negative variance.
        /// </summary>
        [Test]
        public void FromData_NumericInts_ComputesMeanAndNonNegativeVariance()
        {
            int[] data = new[] { 1, 2, 3 };
            var r = UDouble.FromData(data);

            Assert.That(r.Mean, Is.EqualTo(2.0));
            Assert.That(r.Variance, Is.GreaterThanOrEqualTo(0.0));
        }

        /// <summary>
        /// Verifies FromData over float sequence computes mean and positive variance (float rounding behavior).
        /// </summary>
        [Test]
        public void FromData_NumericFloats_UsesFloatRounding()
        {
            float[] data = new[] { 1.0f, 2.0f, 3.0f };
            var r = UDouble.FromData(data);

            Assert.That(r.Mean, Is.EqualTo(2.0));
            Assert.That(r.Variance, Is.GreaterThan(0.0));
        }

        /// <summary>
        /// Verifies FromData over double sequence computes mean and positive variance (double rounding behavior).
        /// </summary>
        [Test]
        public void FromData_NumericDoubles_UsesDoubleRounding()
        {
            double[] data = new[] { 1.0, 2.0, 3.0 };
            var r = UDouble.FromData(data);

            Assert.That(r.Mean, Is.EqualTo(2.0));
            Assert.That(r.Variance, Is.GreaterThan(0.0));
        }

        /// <summary>
        /// Verifies FromData with a single numeric element returns the mean and instrumental variance only.
        /// </summary>
        [Test]
        public void FromData_NumericSingleElement_ReturnsInstrumentalVarianceOnly()
        {
            double[] data = new[] { 5.0 };
            var r = UDouble.FromData(data);

            Assert.That(r.Mean, Is.EqualTo(5.0));
            Assert.That(r.Variance, Is.GreaterThanOrEqualTo(0.0));
        }

        /// <summary>
        /// Verifies FromData throws ArgumentException for an empty numeric sequence.
        /// </summary>
        [Test]
        public void FromData_NumericEmpty_Throws()
        {
            Assert.That(
                () => UDouble.FromData(Array.Empty<int>()),
                Throws.TypeOf<ArgumentException>());
        }

        /// <summary>
        /// Verifies FromData throws ArgumentNullException when passed a null numeric sequence.
        /// </summary>
        [Test]
        public void FromData_NumericNull_Throws()
        {
            Assert.That(
                () => UDouble.FromData<int>(null!),
                Throws.TypeOf<ArgumentNullException>());
        }
        #endregion

        #region // --- Statistical (Welford) and Formatting Tests ---

        /// <summary>
        /// Verifies FromDouble handles subnormal values and uses half-ulp squared variance.
        /// </summary>
        [Test]
        public void FromDouble_Subnormal_UsesHalfUlpSquaredVariance()
        {
            double sub = BitConverter.Int64BitsToDouble(1);
            var x = UDouble.FromDouble(sub);
            var expected = Math.Pow(0.5 * double.Epsilon, 2);
            Assert.That(x.Variance, Is.EqualTo(expected));
        }

        /// <summary>
        /// Verifies FromFloat handles subnormal values and uses half-ulp squared variance.
        /// </summary>
        [Test]
        public void FromFloat_Subnormal_UsesHalfUlpSquaredVariance()
        {
            float sub = BitConverter.Int32BitsToSingle(1);
            var x = UDouble.FromFloat(sub);
            var expected = Math.Pow(0.5 * float.Epsilon, 2);
            Assert.That(x.Variance, Is.EqualTo(expected));
        }

        /// <summary>
        /// Verifies Welford-based FromData remains stable for large numeric values.
        /// </summary>
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
            // Include instrumental variance induced by converting the numeric samples to UDouble via IEEE-754 rounding.
            double sumInst = 0.0;
            for (int i = 0; i < n; i++)
                sumInst += UDouble.FromDouble(data[i]).Variance;
            double avgInst = sumInst / n;

            double expectedVariance = (double)varianceStat + avgInst;

            Assert.That(r.Mean, Is.EqualTo(expectedMean).Within(1e-6));
            Assert.That(r.Variance, Is.EqualTo(expectedVariance).Within(Math.Max(1e-12, Math.Abs(expectedVariance) * 1e-12)));
        }

        /// <summary>
        /// Verifies Welford-based FromData remains stable for large UDouble inputs.
        /// </summary>
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



        /// <summary>
        /// Confirms the public `UDouble.Zero` represents mean 0 and exact zero variance.
        /// </summary>
        [Test]
        public void Zero_IsZeroAndExact()
        {
            Assert.That(UDouble.Zero.Mean, Is.EqualTo(0.0));
            Assert.That(UDouble.Zero.Variance, Is.EqualTo(0.0));
        }

        #endregion

    }
}
