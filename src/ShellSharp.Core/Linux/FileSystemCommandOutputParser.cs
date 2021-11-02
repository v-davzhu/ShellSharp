using ShellSharp.Core.Common;

namespace ShellSharp.Core.Linux
{
    public static class FileSystemCommandOutputParser
    {
        public static List<FileSystemObject> ParseOutput(string output)
        {
            return output.Split('\n', '\r')
                .Select(l => l.Split(' ').Where(s => !String.IsNullOrEmpty(s)).ToArray())
                .Where(l => l.Length > 2)
                .Select(CreateFileSystemObject)
                .ToList();
        }

        private static FileSystemObject CreateFileSystemObject(string[] listSplitted)
        {
            var isDirectory = listSplitted[0][0] == 'd';
            var isLink = listSplitted[0][0] == 'l';
            return new FileSystemObject(
                isDirectory, 
                isLink, 
                listSplitted[0], 
                String.Join("", listSplitted.Skip(8)));
        }
    }
}
