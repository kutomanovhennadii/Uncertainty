namespace Uncertainty.Core
{
    public readonly struct UDouble : IEquatable<UDouble>
    {
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


    }
}
