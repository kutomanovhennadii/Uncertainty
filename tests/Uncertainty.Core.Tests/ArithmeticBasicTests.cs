using NUnit.Framework;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    public sealed class ArithmeticTests
    {
        #region Basic tests
        private static UDouble Exact(double x) => UDouble.FromMeanVar(x, 0.0);

        /// <summary>
        /// Verifies the basic uncertainty arithmetic invariant:
        /// adding two exact values (Variance = 0) must not introduce
        /// artificial uncertainty. The resulting variance must remain zero.
        /// This test also implicitly checks that Mean follows standard
        /// double arithmetic for addition.
        /// </summary>
        [Test]
        public void Exact_Plus_Exact_VarianceIsZero()
        {
            UDouble a = Exact(2.0);
            UDouble b = Exact(3.5);

            UDouble r = a + b;

            Assert.That(r.Mean, Is.EqualTo(5.5));
            Assert.That(r.Variance, Is.EqualTo(0.0));
        }

        /// <summary>
        /// Verifies that multiplying two exact values does not generate
        /// uncertainty. When both operands have zero variance,
        /// the resulting variance must also be zero.
        /// This test validates the multiplication variance formula
        /// in the degenerate exact case.
        /// </summary>
        [Test]
        public void Exact_Multiply_Exact_VarianceIsZero()
        {
            UDouble a = Exact(-2.0);
            UDouble b = Exact(3.5);

            UDouble r = a * b;

            Assert.That(r.Mean, Is.EqualTo(-7.0));
            Assert.That(r.Variance, Is.EqualTo(0.0));
        }

        /// <summary>
        /// Verifies that addition of UDouble values is commutative.
        /// The results of a + b and b + a must be identical
        /// both in Mean and in Variance, confirming the symmetry
        /// of the linear uncertainty propagation formula for addition.
        /// </summary>
        [Test]
        public void Addition_IsCommutative_ForMeanAndVariance()
        {
            UDouble a = UDouble.FromMeanVar(2.0, 0.04);
            UDouble b = UDouble.FromMeanVar(-3.0, 0.09);

            UDouble r1 = a + b;
            UDouble r2 = b + a;

            Assert.That(r2.Mean, Is.EqualTo(r1.Mean));
            Assert.That(r2.Variance, Is.EqualTo(r1.Variance));
        }

        /// <summary>
        /// Verifies that multiplication of UDouble values is commutative.
        /// The results of a * b and b * a must be identical both in Mean
        /// and in Variance, confirming symmetry of the multiplication
        /// uncertainty propagation formula.
        /// </summary>
        [Test]
        public void Multiplication_IsCommutative_ForMeanAndVariance()
        {
            UDouble a = UDouble.FromMeanVar(2.0, 0.04);
            UDouble b = UDouble.FromMeanVar(-3.0, 0.09);

            UDouble r1 = a * b;
            UDouble r2 = b * a;

            Assert.That(r2.Mean, Is.EqualTo(r1.Mean));
            Assert.That(r2.Variance, Is.EqualTo(r1.Variance));
        }

        /// <summary>
        /// Verifies antisymmetry of subtraction for UDouble values.
        /// For a − b and b − a, the Mean values must have opposite signs,
        /// while the Variance must remain identical, reflecting that
        /// uncertainty magnitude is independent of operand order
        /// in subtraction.
        /// </summary>
        [Test]
        public void Subtraction_IsAntisymmetric_ForMeanAndVariance()
        {
            UDouble a = UDouble.FromMeanVar(2.0, 0.04);
            UDouble b = UDouble.FromMeanVar(-3.0, 0.09);

            UDouble r1 = a - b;
            UDouble r2 = b - a;

            Assert.That(r1.Mean, Is.EqualTo(-r2.Mean));
            Assert.That(r1.Variance, Is.EqualTo(r2.Variance));
        }

        /// <summary>
        /// Verifies that the Mean component of UDouble arithmetic
        /// is consistent with standard double arithmetic.
        /// For addition, subtraction, and multiplication, the resulting
        /// Mean must exactly match the corresponding operation
        /// performed on the underlying double values.
        /// </summary>
        [Test]
        public void Mean_IsConsistent_WithDoubleArithmetic()
        {
            UDouble a = UDouble.FromMeanVar(1.25, 0.01);
            UDouble b = UDouble.FromMeanVar(4.00, 0.04);

            Assert.That((a + b).Mean, Is.EqualTo(1.25 + 4.00));
            Assert.That((a - b).Mean, Is.EqualTo(1.25 - 4.00));
            Assert.That((a * b).Mean, Is.EqualTo(1.25 * 4.00));
        }
        #endregion

        #region Variation Tests
        /// <summary>
        /// Verifies the variance formula for addition against a hand-computed reference case.
        /// The test checks that Variance is computed as Va + Vb and that the resulting Mean
        /// matches the exact arithmetic sum.
        /// </summary>
        [Test]
        public void Addition_HandComputedCase_VarianceMatches()
        {
            var a = UDouble.FromMeanVar(2.0, 0.04);
            var b = UDouble.FromMeanVar(3.0, 0.09);

            var r = a + b;

            Assert.That(r.Mean, Is.EqualTo(5.0));
            Assert.That(r.Variance, Is.EqualTo(0.13));
        }

        /// <summary>
        /// Verifies the variance formula for multiplication using a hand-computed reference case.
        /// The test checks that Variance is computed as b²·Va + a²·Vb and that the resulting Mean
        /// matches the exact arithmetic product.
        /// </summary>
        [Test]
        public void Multiplication_HandComputedCase_VarianceMatches()
        {
            var a = UDouble.FromMeanVar(2.0, 0.04);
            var b = UDouble.FromMeanVar(3.0, 0.09);

            var r = a * b;

            Assert.That(r.Mean, Is.EqualTo(6.0));
            Assert.That(r.Variance, Is.EqualTo(0.72));
        }

        /// <summary>
        /// Verifies the variance formula for division using a hand-computed reference case.
        /// The test checks that Variance is computed as Va / b² + a²·Vb / b⁴ and that the resulting
        /// Mean matches the exact arithmetic quotient.
        /// </summary>
        [Test]
        public void Division_HandComputedCase_VarianceMatches()
        {
            var a = UDouble.FromMeanVar(2.0, 0.04);
            var b = UDouble.FromMeanVar(3.0, 0.09);

            var r = a / b;

            Assert.That(r.Mean, Is.EqualTo(2.0 / 3.0));
            Assert.That(r.Variance, Is.EqualTo(0.008888888888888889));
        }
        #endregion

        #region Edge Cases
        /// <summary>
        /// Verifies that division by a value with zero Mean is explicitly forbidden.
        /// The operation must throw <see cref="DivideByZeroException"/>,
        /// confirming correct handling of the undefined arithmetic case
        /// instead of producing an invalid UDouble.
        /// </summary>
        [Test]
        public void Division_ByZero_ThrowsException()
        {
            var a = UDouble.FromMeanVar(1.0, 0.01);
            var b = UDouble.FromMeanVar(0.0, 0.0);

            Assert.That(() => { var _ = a / b; }, Throws.TypeOf<DivideByZeroException>());
        }

        /// <summary>
        /// Verifies numerical stability of division when the raw variance computation
        /// overflows to Infinity. The test ensures that the saturation policy is applied,
        /// resulting in a finite, non-negative Variance, and that the Mean remains finite.
        /// </summary>
        [Test]
        public void Division_VarianceOverflows_IsSaturatedAndFinite()
        {
            // mean is finite; variance computation overflows to Infinity; saturation must clamp to finite
            var a = UDouble.FromMeanVar(1e140, 0.0);
            var b = UDouble.FromMeanVar(1.0, 1e40);

            var r = a / b;

            Assert.That(double.IsFinite(r.Mean), Is.True);
            Assert.That(double.IsFinite(r.Variance), Is.True);
            Assert.That(r.Variance, Is.GreaterThanOrEqualTo(0.0));
        }

        /// <summary>
        /// Verifies correct behavior of the variance saturation policy when the computed
        /// variance exceeds the defined ceiling. The test uses a hand-constructed case
        /// where the relative variance limit dominates, and checks that Variance is
        /// clamped exactly to the expected ceiling value rather than overflowing.
        /// </summary>
        [Test]
        public void Division_VarianceExceedsCeiling_IsClampedToCeiling()
        {
            // Choose mean so that relative ceiling dominates absolute floor but stays finite:
            // mean = 1e143 => mean^2 = 1e286 => relLimit = 1e302 (with factor 1e16)
            // Force computed variance = 1e303 (finite) so it must clamp to 1e302.
            var a = UDouble.FromMeanVar(1e143, 0.0);
            var b = UDouble.FromMeanVar(1.0, 1e17);

            var r = a / b;

            Assert.That(r.Mean, Is.EqualTo(1e143));
            Assert.That(r.Variance, Is.EqualTo(1e302));
        }
        #endregion

    }
}
