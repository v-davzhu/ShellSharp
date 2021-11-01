namespace ShellSharp.Core.Utils
{
    public static class ByteArrayUtils
    {
        public static bool ContainsPattern(
            Span<byte> source,
            Span<byte> pattern,
            int startPosition,
            int length = -1)
        {
            return LocatePattern(source, pattern, startPosition, length) > -1;
        }

        public static int LocatePattern(
            Span<byte> source,
            Span<byte> pattern, 
            int startPosition,
            int length = -1)
        {
            Span<byte> s = source;
            length = length == -1 ? source.Length - startPosition : length + 1;
            var index = LocatePattern(s.Slice(startPosition, length), pattern);
            return index == -1 ? -1 : index + startPosition;
        }

        public static bool ContainsPattern(Span<byte> source, Span<byte> pattern)
        {
            return LocatePattern(source, pattern) > -1;
        }

        public static int LocatePattern(Span<byte> source, Span<byte> pattern)
        {
            Span<byte> bytes = source;
            for (int i = 0; i < source.Length - pattern.Length + 1; i++)
            {
                if (IsEqual(bytes.Slice(i, pattern.Length), pattern)) 
                {
                    return i;
                }
            }

            return -1;
        }

        private static bool IsEqual(Span<byte> span, Span<byte> pattern)
        {
            for (int i = 0; i < pattern.Length; i++)
            {
                if (span[i] != pattern[i])
                    return false;
            }

            return true;
        }
    }
}
