using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mandro.Utils
{
    public static class StreamExtensions
    {
            public static byte[] ReadToEnd(this Stream sourceStream)
            {
                using (var memoryStream = new MemoryStream())
                {
                    sourceStream.CopyTo(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            public static string ReadString(this Stream sourceStream)
            {
                return Encoding.UTF8.GetString(ReadToEnd(sourceStream));
            }

            public static async Task<byte[]> ReadToEndAsync(this Stream sourceStream)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await sourceStream.CopyToAsync(memoryStream);
                    return memoryStream.ToArray();
                }
            }

            public static async Task<string> ReadStringAsync(this Stream sourceStream)
            {
                return Encoding.UTF8.GetString(await ReadToEndAsync(sourceStream));
            }
        } 
}