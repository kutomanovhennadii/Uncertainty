using System;

namespace Uncertainty.Core.Policies
{
    /// <summary>
    /// Throwing strategy: small denominators are treated as errors and cause exceptions.
    /// </summary>
    internal sealed class ThrowingDivisionStrategy : IDivisionStrategy
    {
        public UDouble HandleSmallDenominator(UDouble a, UDouble b)
        {
            throw new DivideByZeroException("Denominator mean is too close to zero.");
        }
    }

    /// <summary>
    /// Saturating strategy: perform the division and apply variance saturation to keep results finite.
    /// </summary>
    internal sealed class SaturatingDivisionStrategy : IDivisionStrategy
    {
        public UDouble HandleSmallDenominator(UDouble a, UDouble b)
        {
            double mean = a.Mean / b.Mean;

            double b2 = b.Mean * b.Mean;
            double b4 = b2 * b2;

            double variance =
                a.Variance / b2 +
                (a.Mean * a.Mean * b.Variance) / b4;

            variance = VarianceSaturationPolicy.SaturateVariance(mean, variance);

            return UDouble.FromMeanVar(mean, variance);
        }
    }

    /// <summary>
    /// Return-infinity strategy: allow mean to become ±Infinity and saturate variance to a finite ceiling.
    /// </summary>
    internal sealed class ReturnInfinityDivisionStrategy : IDivisionStrategy
    {
        public UDouble HandleSmallDenominator(UDouble a, UDouble b)
        {
            // Currently, returning ±Infinity mean violates core contracts (mean must be finite).
            // Therefore treat this policy as equivalent to SaturateVariance for numeric robustness.
            double mean = a.Mean / b.Mean;

            double b2 = b.Mean * b.Mean;
            double b4 = b2 * b2;

            double variance =
                a.Variance / b2 +
                (a.Mean * a.Mean * b.Variance) / b4;

            variance = VarianceSaturationPolicy.SaturateVariance(mean, variance);

            return UDouble.FromMeanVar(mean, variance);
        }
    }
}
