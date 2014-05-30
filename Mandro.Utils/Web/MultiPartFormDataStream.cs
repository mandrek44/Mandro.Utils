using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Mandro.Utils.Web
{
    /// <summary>
    /// Stream used to read multipart/form-data.
    /// See SeekNextFile.
    /// </summary>
    public class MultiPartFormDataStream : Stream
    {
        private readonly PatternLimitedStream _stream;
        private readonly string _multiPartBoundary;
        private readonly PatternLimitedStreamReader _streamReader;

        private string _currentFileName;
        private string _currentFormName;
        private string _currentContentType;

        private bool _initialized;

        public MultiPartFormDataStream(Stream stream, string contentType)
        {
            if (!contentType.StartsWith("multipart/form-data"))
                throw new ArgumentException("Content type must be multipart/form", "contentType");

            _stream = new PatternLimitedStream(stream);
            _multiPartBoundary = Regex.Match(contentType, @"boundary=""?(?<boundary>[^\n\;\"" ]*)").Groups["boundary"].Value;
            _streamReader = new PatternLimitedStreamReader(_stream, Encoding.UTF8);
        }

        /// <summary>
        /// Places the stream at the beginning of next file data, and populates the CurrentContentType, CurrentFileName and CurrentFormName properties.
        /// The Read method won't read pass the end of the file. When end of file is reached, you must call SeekNextFile again.
        /// </summary>
        /// <returns>true if next File has been found in Stream, false otherwise.</returns>
        public bool SeekNextFile()
        {
            if (!_initialized)
            {
                _streamReader.ReadUntil(_multiPartBoundary);
                _initialized = true;
            }

            var untilNextLine = _streamReader.ReadUntil("\n");
            if (string.IsNullOrEmpty(untilNextLine) || untilNextLine.Trim() == "--")
            {
                return false;
            }

            _currentFileName = string.Empty;
            while (true)
            {
                var header = _streamReader.ReadUntil("\n").Trim();

                if (string.IsNullOrEmpty(header))
                {
                    break;
                }

                if (header.StartsWith("Content-Type", StringComparison.InvariantCultureIgnoreCase))
                {
                    _currentContentType = header.Split(new[] { ' ' }).Last().Trim();
                }
                else if (header.StartsWith("Content-Disposition", StringComparison.CurrentCultureIgnoreCase))
                {
                    _currentFileName = Regex.Match(header, @"filename=""?(?<fileName>[^\""]*)", RegexOptions.IgnoreCase).Groups["fileName"].Value;
                    _currentFormName = Regex.Match(header, @" name=""?(?<name>[^\""]*)", RegexOptions.IgnoreCase).Groups["name"].Value;
                }
            }

            _stream.SetPattern("\r\n--" + _multiPartBoundary);
            return true;
        }

        public override void Flush()
        {
            _stream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void SetLength(long value)
        {
            _stream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public override bool CanRead
        {
            get
            {
                return _stream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return _stream.CanSeek;
            }
        }

        public override bool CanTimeout
        {
            get
            {
                return _stream.CanTimeout;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return _stream.CanWrite;
            }
        }

        public override long Length
        {
            get
            {
                return _stream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _stream.Position;
            }
            set
            {
                _stream.Position = value;
            }
        }

        public string CurrentFileName
        {
            get
            {
                return _currentFileName;
            }
        }

        public string CurrentFormName
        {
            get
            {
                return _currentFormName;
            }
        }

        public string CurrentContentType
        {
            get
            {
                return _currentContentType;
            }
        }
    }
}