using System.IO;
using System.Text;

namespace ShellSharp.Core.Utils
{
    public static class LinuxShellUtils
    {
        public static byte[] EndCommandSequence = new byte[] { 0x0A, 0x0A };

        /// <summary>
        /// When a shell answer it will echo the original command, then 
        /// after a double 0x0a char it will present prompt.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static Span<byte> ParseShellAnswer(MemoryStream ms) 
        {
            var array = ms.ToArray();
            var startLine = Array.IndexOf<byte>(array, 0x0A);
            var endLine = ByteArrayUtils.LocatePattern(array, LinuxShellUtils.EndCommandSequence, startLine + 1);

            if (endLine == -1)
            {
                endLine = array.Length;
            }
            return ((Span<byte>) array).Slice(startLine + 1, endLine - 1 - startLine);
        }

        /// <summary>
        /// Verify if the memorystream has only one line.
        /// </summary>
        /// <param name="ms"></param>
        /// <returns></returns>
        public static bool IsSingleLine(MemoryStream ms) 
        {
            return ms.ToArray().Count(c => c == 0x0A) == 0;
        }
    }
}
