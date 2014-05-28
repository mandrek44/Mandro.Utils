using System.IO;
using System.Text;

namespace Mandro.Utils
{
    /// <summary>
    /// Class that exposes methods to read consecutive data from PatternLimitedStream.
    /// </summary>
    public class PatternLimitedStreamReader
    {
        private readonly PatternLimitedStream _patternLimitedStream;
        private readonly Encoding _encoding;

        public PatternLimitedStreamReader(PatternLimitedStream patternLimitedStream, Encoding encoding)
        {
            _patternLimitedStream = patternLimitedStream;
            _encoding = encoding;
        }

        public byte[] ReadUntil(byte[] pattern)
        {
            _patternLimitedStream.SetPattern(pattern);

            return ReadFully(_patternLimitedStream);
        }

        public string ReadUntil(string pattern)
        {
            return _encoding.GetString(ReadUntil(_encoding.GetBytes(pattern)));
        }

        private static byte[] ReadFully(Stream input)
        {
            var buffer = new byte[16 * 1024];
            using (var ms = new MemoryStream())
            {
                int read;
                while ((read = input.Read(buffer, 0, buffer.Length)) > 0)
                {
                    ms.Write(buffer, 0, read);
                }
                return ms.ToArray();
            }
        }
    }
}