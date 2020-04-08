using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.FileService
{
    public class StreamFile : IFile
    {
        private readonly Stream _s;
        private readonly string _mimeType;

        public StreamFile(Stream s, string mimeType)
        {
            _s = s;
            _mimeType = mimeType;
        }

        public byte[] AsBytes()
        {
            using var ms = new MemoryStream();
            _s.CopyTo(ms);
            return ms.ToArray();
        }

        public Stream AsStream()
        {
            return _s;
        }

        public async Task<byte[]> AsBytesAsync()
        {
            await using var ms = new MemoryStream();
            await _s.CopyToAsync(ms);
            return ms.ToArray();
        }

        public async Task<string> GetMimeTypeAsync()
        {
            return _mimeType;
        }

        public async Task<long> GetLength()
        {
            return _s.Length;
        }
    }
}
