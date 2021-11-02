using NUnit.Framework;
using ShellSharp.Core.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShellSharp.Tests.Core.Utils
{
    [TestFixture]
    public class StringBuilderUtilsTests : BaseTests
    {
        [Test]
        public void RemoveFirstLine() 
        {
            var data = ReadTestDataByteArray("linux\\shellanswer.bin");
            using var ms = new MemoryStream(data);
            var span = LinuxShellUtils.ParseShellAnswer(ms);

            var decoded = Encoding.UTF8.GetString(span);
            Assert.That(decoded.StartsWith("bin"));
            Assert.That(decoded.EndsWith("var"));
        }
    }
}
