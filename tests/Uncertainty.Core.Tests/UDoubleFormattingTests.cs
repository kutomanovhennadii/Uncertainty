using NUnit.Framework;
using System.Globalization;

namespace Uncertainty.Core.Tests
{
    [TestFixture]
    public sealed class UDoubleFormattingTests
    {
        /// <summary>
        /// Verifies default and formatted ToString produce expected representations (invariant culture).
        /// </summary>
        [Test]
        public void ToString_DefaultAndFormat_ReturnsExpected()
        {
            var u = UDouble.FromMeanVar(1.2345, 0.25);

            var defaultStr = u.ToString(null, CultureInfo.InvariantCulture);
            Assert.That(defaultStr, Is.EqualTo("1.2345 ± 0.5"));

            var f2 = u.ToString("F2", CultureInfo.InvariantCulture);
            Assert.That(f2, Is.EqualTo("1.23 ± 0.50"));
        }

        /// <summary>
        /// Verifies ToString uses the current thread culture when no provider is specified.
        /// </summary>
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
    }
}
