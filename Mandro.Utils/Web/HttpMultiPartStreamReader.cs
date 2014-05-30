using System.Collections.Generic;
using System.IO;

namespace Mandro.Utils.Web
{
    public class HttpMultiPartStreamReader
    {
        /// <summary>
        /// Read all files from multipart/form-data stream.
        /// </summary>
        /// <param name="stream">Source multipart/form-data stream.</param>
        /// <returns>Enumeration of each file, together with its content.</returns>
        public static IEnumerable<File> GetFiles(MultiPartFormDataStream stream)
        {
            while (stream.SeekNextFile())
            {
                yield return new File { Content = ReadFully(stream), FileName = stream.CurrentFileName, FormName = stream.CurrentFormName, ContentType = stream.CurrentContentType };
            }
        }

        public class File
        {
            public byte[] Content { get; set; }

            public string FileName { get; set; }

            public string FormName { get; set; }

            public string ContentType { get; set; }
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