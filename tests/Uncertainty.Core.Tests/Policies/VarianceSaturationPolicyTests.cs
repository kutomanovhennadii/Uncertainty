using NUnit.Framework;
using Uncertainty.Core.Policies.VarianceSaturationPolicies;

namespace Uncertainty.Core.Tests.Policies
{
    [TestFixture]
    public sealed class VarianceSaturationPolicyTests
    {
        /// <summary>
        /// Validates the saturation matrix: different mean/variance combinations yield expected ceiling behavior.
        /// </summary>
        [Test]
        public void SaturateVariance_MatrixBehavesAsSpecified()
        {
            double[] means = { 0.0, 1e-308, 1e150, double.PositiveInfinity };
            double[] variances = { double.NaN, double.PositiveInfinity, 1e299, double.MaxValue, 1e300 - 1e290 };

            foreach (var mean in means)
            {
                foreach (var variance in variances)
                {
                    double result = VarianceSaturationPolicy.SaturateVariance(mean, variance);

                    double relLimit;
                    double absMean = System.Math.Abs(mean);
                    if (double.IsFinite(absMean))
                    {
                        double meanSq = absMean * absMean;
                        relLimit = meanSq * VarianceSaturationPolicy.MaxRelativeVarianceFactor;

                        if (!double.IsFinite(relLimit) || relLimit <= 0.0)
                        {
                            relLimit = VarianceSaturationPolicy.AbsoluteVarianceMax;
                        }
                    }
                    else
                    {
                        relLimit = VarianceSaturationPolicy.AbsoluteVarianceMax;
                    }

                    double ceiling = System.Math.Max(relLimit, VarianceSaturationPolicy.AbsoluteVarianceMax);

                    double expected = (!double.IsFinite(variance) || variance > ceiling) ? ceiling : variance;

                    Assert.That(result, Is.EqualTo(expected), $"mean={mean}, var={variance}");
                }
            }
        }
    }
}
