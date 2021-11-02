namespace ShellSharp.Core.Common
{
    public class FileSystemObject
    {
        public FileSystemObject(bool isDirectory, bool isLink, string permissions, string name)
        {
            IsDirectory = isDirectory;
            IsLink = isLink;
            Permissions = permissions;
            Name = name;
        }

        public bool IsDirectory { get; set; }

        public bool IsLink { get; set; }

        public string Permissions { get; set; }

        public string Name { get; set; }
    }
}
