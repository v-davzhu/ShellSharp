using NUnit.Framework;
using ShellSharp.Core.Utils;

namespace ShellSharp.Tests.Core.Utils
{
    [TestFixture]
    public class ByteArrayUtilsTests
    {
        [Test]
        public void Locate_pattern()
        {
            byte[] array = new byte[] { 1, 2, 3, 4, 5, 6 };
            byte[] pattern = new byte[] { 3, 4, };

            Assert.That(ByteArrayUtils.LocatePattern(array, pattern), Is.EqualTo(2));
        }

        [Test]
        public void Locate_pattern_at_end()
        {
            byte[] array = new byte[] { 1, 2, 3, 4, 5, 6 };
            byte[] pattern = new byte[] { 5,6, };

            Assert.That(ByteArrayUtils.LocatePattern(array, pattern), Is.EqualTo(4));
        }

        [Test]
        public void Locate_pattern_at_start()
        {
            byte[] array = new byte[] { 1, 2, 3, 4, 5, 6 };
            byte[] pattern = new byte[] { 1, 2, };

            Assert.That(ByteArrayUtils.LocatePattern(array, pattern), Is.EqualTo(0));
        }

        [Test]
        public void Locate_pattern_no_match()
        {
            byte[] array = new byte[] { 1, 2, 3, 4, 5, 6 };
            byte[] pattern = new byte[] { 3, 5, };

            Assert.That(ByteArrayUtils.LocatePattern(array, pattern), Is.EqualTo(-1));
        }

        [Test]
        public void Locate_pattern_starting_at()
        {
            byte[] array = new byte[] { 1, 2, 3, 4, 5, 3, 4, 6 };
            byte[] pattern = new byte[] { 3, 4, };

            Assert.That(ByteArrayUtils.LocatePattern(array, pattern, 3), Is.EqualTo(5));
        }

        [Test]
        public void Locate_pattern_starting_at_not_found()
        {
            byte[] array = new byte[] { 1, 2, 3, 4, 5, 3, 4, 6 };
            byte[] pattern = new byte[] { 3, 45, };

            Assert.That(ByteArrayUtils.LocatePattern(array, pattern, 3), Is.EqualTo(-1));
        }

        [Test]
        public void Locate_pattern_starting_at_ending_at()
        {
            byte[] array = new byte[] { 1, 2, 5, 4, 5, 3, 4, 6 };
            byte[] pattern = new byte[] { 3, 4, };

            Assert.That(ByteArrayUtils.LocatePattern(array, pattern, 0, 5), Is.EqualTo(-1));
            Assert.That(ByteArrayUtils.LocatePattern(array, pattern, 0, 6), Is.EqualTo(5));
        }
    }
}
