using System;
using System.IO;

namespace ShellSharp.Tests
{
    public class BaseTests
    {
        protected byte[] ReadTestDataByteArray(string path)
        {
            string dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", path);
            return File.ReadAllBytes(dataFilePath);
        }

        protected string ReadTestDataString(string path)
        {
            string dataFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "data", path);
            return File.ReadAllText(dataFilePath);
        }
    }
}
