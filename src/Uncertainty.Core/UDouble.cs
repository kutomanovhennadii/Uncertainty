using System.Numerics;

namespace Uncertainty.Core
{
    public readonly struct UDouble : IEquatable<UDouble>
    {
        #region Properties
        /// <summary>
        /// Gets the expected value (mean) of the underlying normal distribution.
        /// This is the numeric value used for ordering and comparison operations.
        /// </summary>
        public double Mean { get; }

        /// <summary>
        /// Gets the variance (σ²) of the underlying normal distribution.
        /// Variance must always be a finite non-negative value.
        /// </summary>
        public double Variance { get; }

        /// <summary>
        /// Gets the standard deviation (σ), computed as the square root of <see cref="Variance"/>.
        /// This value is derived and never stored.
        /// </summary>
        public double StdDev => Math.Sqrt(Variance);
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="UDouble"/> type with explicit
        /// <paramref name="mean"/> and <paramref name="variance"/>.
        /// </summary>
        /// <param name="mean">The expected value of the distribution. Must be finite.</param>
        /// <param name="variance">The variance of the distribution. Must be finite and non-negative.</param>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="mean"/> is NaN or infinite,
        /// or when <paramref name="variance"/> is NaN, infinite, or negative.
        /// </exception>
        internal UDouble(double mean, double variance)
        {
            if (double.IsNaN(mean) || double.IsInfinity(mean))
                throw new ArgumentException("Mean must be finite.", nameof(mean));

            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0)
                throw new ArgumentException("Variance must be finite and ≥ 0.", nameof(variance));

            Mean = mean;
            Variance = variance;
        }
        #endregion

        #region Factories
        /// <summary>
        /// Creates a <see cref="UDouble"/> from a specified mean and variance.
        /// Variance must be a finite non-negative value. No automatic adjustments are performed.
        /// </summary>
        /// <param name="mean">The expected value of the distribution. Must be finite.</param>
        /// <param name="variance">The variance (σ²). Must be finite and ≥ 0.</param>
        /// <returns>A new <see cref="UDouble"/> instance.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="mean"/> or <paramref name="variance"/> is NaN or infinite,
        /// or when <paramref name="variance"/> is negative.
        /// </exception>
        public static UDouble FromMeanVar(double mean, double variance)
        {
            if (double.IsNaN(mean) || double.IsInfinity(mean))
                throw new ArgumentException("Mean must be finite.", nameof(mean));

            if (double.IsNaN(variance) || double.IsInfinity(variance) || variance < 0)
                throw new ArgumentException("Variance must be finite and ≥ 0.", nameof(variance));

            return new UDouble(mean, variance);
        }

        /// <summary>
        /// Creates a <see cref="UDouble"/> from a specified mean and standard deviation.
        /// The standard deviation is converted to variance as <c>stdDev * stdDev</c>.
        /// No automatic adjustments are performed.
        /// </summary>
        /// <param name="mean">The expected value of the distribution. Must be finite.</param>
        /// <param name="stdDev">The standard deviation (σ). Must be finite and ≥ 0.</param>
        /// <returns>A new <see cref="UDouble"/> instance.</returns>
        /// <exception cref="ArgumentException">
        /// Thrown when <paramref name="mean"/> or <paramref name="stdDev"/> is NaN or infinite,
        /// or when <paramref name="stdDev"/> is negative.
        /// </exception>
        public static UDouble FromMeanStd(double mean, double stdDev)
        {
            if (double.IsNaN(mean) || double.IsInfinity(mean))
                throw new ArgumentException("Mean must be finite.", nameof(mean));

            if (double.IsNaN(stdDev) || double.IsInfinity(stdDev) || stdDev < 0)
                throw new ArgumentException("Standard deviation must be finite and ≥ 0.", nameof(stdDev));

            return FromMeanVar(mean, stdDev * stdDev);
        }

        /// <summary>
        /// Creates a <see cref="UDouble"/> from a <see cref="double"/>, assigning uncertainty from IEEE-754 rounding.
        /// The variance is computed as <c>(0.5 * ulp(x))^2</c>.
        /// </summary>
        /// <param name="x">The input double value. Must be finite.</param>
        /// <returns>A new <see cref="UDouble"/> instance with mean <paramref name="x"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="x"/> is NaN or infinite.
        /// </exception>
        public static UDouble FromDouble(double x)
        {
            if (double.IsNaN(x) || double.IsInfinity(x))
                throw new ArgumentOutOfRangeException(nameof(x), "Value must be finite.");

            // IEEE-754: ulp(x) = 2^(e - 52) for normal numbers, where e is the unbiased exponent.
            // For subnormals, ulp is constant: 2^-1074.
            double ulp;
            if (x == 0.0)
            {
                ulp = double.Epsilon; // 2^-1074
            }
            else
            {
                long bits = BitConverter.DoubleToInt64Bits(x);
                int exponentBits = (int)((bits >> 52) & 0x7FF);

                if (exponentBits == 0)
                {
                    // Subnormal
                    ulp = double.Epsilon; // 2^-1074
                }
                else
                {
                    int unbiasedExponent = exponentBits - 1023;
                    ulp = Math.ScaleB(1.0, unbiasedExponent - 52); // 2^(e-52)
                }
            }

            double halfUlp = 0.5 * ulp;
            double variance = halfUlp * halfUlp;

            return FromMeanVar(x, variance);
        }

        /// <summary>
        /// Creates a <see cref="UDouble"/> from a <see cref="float"/>, assigning uncertainty from IEEE-754 rounding.
        /// The variance is computed as <c>(0.5 * ulp(x))^2</c> using the float ULP.
        /// </summary>
        /// <param name="x">The input float value. Must be finite.</param>
        /// <returns>A new <see cref="UDouble"/> instance with mean equal to <paramref name="x"/>.</returns>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Thrown when <paramref name="x"/> is NaN or infinite.
        /// </exception>
        public static UDouble FromFloat(float x)
        {
            if (float.IsNaN(x) || float.IsInfinity(x))
                throw new ArgumentOutOfRangeException(nameof(x), "Value must be finite.");

            // IEEE-754 float: ulp(x) = 2^(e - 23) for normal numbers, where e is the unbiased exponent.
            // For subnormals (including 0), ulp is constant: 2^-149.
            float ulp;
            if (x == 0f)
            {
                ulp = float.Epsilon; // 2^-149
            }
            else
            {
                int bits = BitConverter.SingleToInt32Bits(x);
                int exponentBits = (bits >> 23) & 0xFF;

                if (exponentBits == 0)
                {
                    ulp = float.Epsilon; // subnormal
                }
                else
                {
                    int unbiasedExponent = exponentBits - 127;
                    ulp = MathF.ScaleB(1f, unbiasedExponent - 23); // 2^(e-23)
                }
            }

            double halfUlp = 0.5 * (double)ulp;
            double variance = halfUlp * halfUlp;

            return FromMeanVar(x, variance);
        }

        /// <summary>
        /// Creates a <see cref="UDouble"/> from any numeric type using IEEE-754 semantics.
        /// float uses float ULP, double uses double ULP, all other numeric types are
        /// converted to double and processed via double ULP.
        /// </summary>
        /// <typeparam name="T">Any numeric type.</typeparam>
        /// <param name="value">Numeric value.</param>
        /// <returns>A <see cref="UDouble"/> instance.</returns>
        public static UDouble FromNumber<T>(T value) where T : INumber<T>
        {
            if (typeof(T) == typeof(float))
                return FromFloat((float)(object)value);

            if (typeof(T) == typeof(double))
                return FromDouble((double)(object)value);

            // All other numeric types: convert to double and apply IEEE-754 double model.
            double dv = double.CreateChecked(value);
            return FromDouble(dv);
        }

#pragma warning disable CA2225 // Operator overloads have named alternates
        /// <summary>
        /// Explicitly converts a <see cref="double"/> to <see cref="UDouble"/> using
        /// IEEE-754 rounding semantics via <see cref="FromDouble(double)"/>.
        /// </summary>
        public static explicit operator UDouble(double x)
            => FromDouble(x);

        /// <summary>
        /// Explicitly converts a <see cref="float"/> to <see cref="UDouble"/> using
        /// IEEE-754 rounding semantics via <see cref="FromFloat(float)"/>.
        /// </summary>
        public static explicit operator UDouble(float x)
            => FromFloat(x);

        /// <summary>
        /// Explicitly converts an <see cref="int"/> to <see cref="UDouble"/> by first
        /// converting to <see cref="double"/> and then applying IEEE-754 rounding
        /// semantics via <see cref="FromDouble(double)"/>.
        /// </summary>
        public static explicit operator UDouble(sbyte x) => FromNumber(x);
        public static explicit operator UDouble(byte x) => FromNumber(x);
        public static explicit operator UDouble(short x) => FromNumber(x);
        public static explicit operator UDouble(ushort x) => FromNumber(x);
        public static explicit operator UDouble(int x) => FromNumber(x);
        public static explicit operator UDouble(uint x) => FromNumber(x);
        public static explicit operator UDouble(long x) => FromNumber(x);
        public static explicit operator UDouble(ulong x) => FromNumber(x);

        public static explicit operator UDouble(nint x) => FromNumber(x);
        public static explicit operator UDouble(nuint x) => FromNumber(x);

        public static explicit operator UDouble(decimal x) => FromNumber(x);
#pragma warning restore CA2225

        /// <summary>
        /// Creates a <see cref="UDouble"/> from a dataset of <see cref="UDouble"/> samples,
        /// combining statistical uncertainty of the mean with the average instrumental uncertainty.
        /// </summary>
        /// <param name="data">Input samples. Each element contributes its <see cref="UDouble.Mean"/> and <see cref="UDouble.Variance"/>.</param>
        /// <returns>
        /// A <see cref="UDouble"/> whose mean is the arithmetic average of sample means and whose variance is:
        /// variance_total = variance_stat + variance_inst,
        /// where variance_stat is the variance of the sample mean (computed from sample means) and
        /// variance_inst is the average of sample variances.
        /// </returns>
        /// <remarks>
        /// Algorithm:
        /// mean = Average(Meanᵢ)
        /// sigma²_stat = Σ(Meanᵢ - mean)² / (n - 1)  (for n &gt; 1, else 0)
        /// variance_stat = sigma²_stat / n
        /// variance_inst = Average(Varianceᵢ)
        /// variance_total = variance_stat + variance_inst
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="data"/> is empty.</exception>
        public static UDouble FromData(IEnumerable<UDouble> data)
        {
            ArgumentNullException.ThrowIfNull(data);

            var samples = data as IList<UDouble> ?? data.ToList();
            int n = samples.Count;
            if (n == 0) throw new ArgumentException("Data must contain at least one element.", nameof(data));

            double mean = samples.Average(s => s.Mean);
            double varianceInst = samples.Average(s => s.Variance);

            double sigma2Stat = 0.0;
            if (n > 1)
            {
                double sumSq = 0.0;
                for (int i = 0; i < n; i++)
                {
                    double d = samples[i].Mean - mean;
                    sumSq += d * d;
                }
                sigma2Stat = sumSq / (n - 1);
            }

            double varianceStat = sigma2Stat / n;
            return FromMeanVar(mean, varianceStat + varianceInst);
        }

        /// <summary>
        /// Creates a <see cref="UDouble"/> from a dataset of numeric samples, applying IEEE-754 rounding
        /// semantics to each element and then combining statistical uncertainty of the mean with
        /// the average instrumental uncertainty induced by conversion.
        /// </summary>
        /// <typeparam name="T">A numeric type implementing <see cref="INumber{T}"/>.</typeparam>
        /// <param name="data">Input samples.</param>
        /// <returns>
        /// A <see cref="UDouble"/> whose mean is the arithmetic average of converted sample means and whose variance is:
        /// variance_total = variance_stat + variance_inst,
        /// where variance_stat is the variance of the sample mean (computed from converted means) and
        /// variance_inst is the average of converted sample variances.
        /// </returns>
        /// <remarks>
        /// Each element is first converted via <see cref="FromNumber{T}(T)"/>, which assigns variance from IEEE-754 rounding
        /// (float uses float ULP, double uses double ULP, other numeric types are converted to double and processed via double ULP).
        /// The dataset aggregation then follows:
        /// mean = Average(Meanᵢ)
        /// sigma²_stat = Σ(Meanᵢ - mean)² / (n - 1)  (for n &gt; 1, else 0)
        /// variance_stat = sigma²_stat / n
        /// variance_inst = Average(Varianceᵢ)
        /// variance_total = variance_stat + variance_inst
        /// </remarks>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="data"/> is null.</exception>
        /// <exception cref="ArgumentException">Thrown when <paramref name="data"/> is empty.</exception>
        public static UDouble FromData<T>(IEnumerable<T> data) where T : INumber<T>
        {
            ArgumentNullException.ThrowIfNull(data);

            var list = data as IList<T> ?? data.ToList();
            int n = list.Count;
            if (n == 0) throw new ArgumentException("Data must contain at least one element.", nameof(data));

            var samples = new UDouble[n];
            for (int i = 0; i < n; i++)
                samples[i] = FromNumber(list[i]);

            double mean = samples.Average(s => s.Mean);
            double varianceInst = samples.Average(s => s.Variance);

            double sigma2Stat = 0.0;
            if (n > 1)
            {
                double sumSq = 0.0;
                for (int i = 0; i < n; i++)
                {
                    double d = samples[i].Mean - mean;
                    sumSq += d * d;
                }
                sigma2Stat = sumSq / (n - 1);
            }

            double varianceStat = sigma2Stat / n;
            return FromMeanVar(mean, varianceStat + varianceInst);
        }
        #endregion

        #region Equals
        /// <summary>
        /// Determines structural equality between two <see cref="UDouble"/> values.
        /// Two instances are equal only if both <see cref="Mean"/> and <see cref="Variance"/> match exactly.
        /// This reflects identity of a measurement rather than numeric equality.
        /// </summary>
        public bool Equals(UDouble other)
            => Mean == other.Mean && Variance == other.Variance;

        /// <summary>
        /// Determines structural equality with another object.
        /// Returns <c>true</c> only if the object is a <see cref="UDouble"/> and both
        /// <see cref="Mean"/> and <see cref="Variance"/> match exactly.
        /// </summary>
        public override bool Equals(object? obj)
            => obj is UDouble other && Equals(other);

        /// <summary>
        /// Computes a hash code based on both <see cref="Mean"/> and <see cref="Variance"/>.
        /// Equal <see cref="UDouble"/> instances always produce identical hash codes.
        /// </summary>
        public override int GetHashCode()
            => HashCode.Combine(Mean, Variance);

        /// <summary>
        /// Structural equality operator. Returns <c>true</c> only when both
        /// <see cref="Mean"/> and <see cref="Variance"/> are identical.
        /// </summary>
        public static bool operator ==(UDouble left, UDouble right)
            => left.Equals(right);

        /// <summary>
        /// Structural inequality operator. Logical negation of <see cref="op_Equality"/>.
        /// </summary>
        public static bool operator !=(UDouble left, UDouble right)
            => !left.Equals(right);
        #endregion

    }
}
