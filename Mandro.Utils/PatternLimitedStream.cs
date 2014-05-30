using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Mandro.Utils
{
    /// <summary>
    /// Stream decorator that ends when pattern is found.
    /// To continue reading call SearchAgain or SetPattern.
    /// </summary>
    public class PatternLimitedStream : Stream
    {
        private readonly Stream _baseStream;
        private byte[] _pattern;
        private readonly List<byte> _searchBuffer = new List<byte>();
        private bool _finished;

        public PatternLimitedStream(Stream baseStream)
        {
            _baseStream = baseStream;
        }

        /// <summary>
        /// Sets the pattern. Stream will end when pattern is found.
        /// </summary>
        /// <param name="pattern"></param>
        public void SetPattern(byte[] pattern)
        {
            _pattern = pattern;
            _finished = false;
        }

        /// <summary>
        /// Sets the pattern. It's assumed that pattern is UTF8 encoded.
        /// Stream will end when pattern is found.
        /// </summary>
        /// <param name="pattern"></param>
        public void SetPattern(string pattern)
        {
            SetPattern(Encoding.UTF8.GetBytes(pattern));
        }

        /// <summary>
        /// Resets the stream so the pattern will be searched again.
        /// </summary>
        public void SearchAgain()
        {
            _finished = false;
        }


        public override void Flush()
        {
            _baseStream.Flush();
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotImplementedException();
        }

        public override void SetLength(long value)
        {
            _baseStream.SetLength(value);
        }

        public override int Read(byte[] buffer, int offset, int requestedCount)
        {
            // If pattern match is disabled but there's some leftover in search buffer
            // then we can't read directly from underlaying stream.
            var isPatternMatchEnabled = _pattern != null && _pattern.Length != 0;
            if (!isPatternMatchEnabled && _searchBuffer.Count == 0)
            {
                return _baseStream.Read(buffer, offset, requestedCount);
            }

            // If pattern was already found, do nothing until it's reset.
            if (isPatternMatchEnabled && _finished) return 0;

            var patternLength = isPatternMatchEnabled ? _pattern.Length : 0;

            
            var internalRequestedCount = requestedCount + patternLength - _searchBuffer.Count;
            var internalBuffer = new byte[internalRequestedCount];
            var bytesRead = _baseStream.Read(internalBuffer, 0, internalRequestedCount);
            if (bytesRead == 0 && _searchBuffer.Count == 0) return 0;

            _searchBuffer.AddRange(internalBuffer.Take(bytesRead));

            // Check if the pattern is present in search buffer. 
            var find = isPatternMatchEnabled ? _searchBuffer.IndexOf(_pattern) : -1;
            if (find == -1)
            {
                // Leave the patternLength bytes in search buffer
                var bytesToCopy = Math.Min(_searchBuffer.Count - patternLength, requestedCount);

                _searchBuffer.CopyTo(0, buffer, offset, bytesToCopy);
                _searchBuffer.RemoveRange(0, bytesToCopy);

                return bytesToCopy;
            }
            else
            {
                // When pattern is found, remove it from search buffer
                _finished = true;
                _searchBuffer.CopyTo(0, buffer, offset, find);
                _searchBuffer.RemoveRange(0, find + patternLength);
                return find;
            }
        }

        /// <summary>
        /// Not implemented in Pattern Limited Stream.
        /// </summary>
        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotImplementedException();
        }

        public override bool CanRead
        {
            get
            {
                return _baseStream.CanRead;
            }
        }

        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        public override bool CanWrite
        {
            get
            {
                return false;
            }
        }

        public override long Length
        {
            get
            {
                return _baseStream.Length;
            }
        }

        public override long Position
        {
            get
            {
                return _baseStream.Position;
            }
            set
            {
                _baseStream.Position = value;
            }
        }
    }
}