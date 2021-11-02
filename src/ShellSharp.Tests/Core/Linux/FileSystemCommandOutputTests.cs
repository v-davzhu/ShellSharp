using NUnit.Framework;
using ShellSharp.Core.Linux;

namespace ShellSharp.Tests.Core.Linux
{
    [TestFixture]
    public class FileSystemCommandOutputTests : BaseTests
    {
        [Test]
        public void Basic_parsing_of_ls_command() 
        {
            var data = base.ReadTestDataString("linux/LsOutput1.txt");
            var parsed = FileSystemCommandOutputParser.ParseOutput(data);
            Assert.That(parsed.Count, Is.EqualTo(23));
            var first = parsed[0];
            Assert.That(first.IsLink);
            Assert.That(first.Permissions, Is.EqualTo("lrwxrwxrwx"));
            Assert.That(first.Name, Is.EqualTo("bin -> usr/bin"));
        }
    }
}
