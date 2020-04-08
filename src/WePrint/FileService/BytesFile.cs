using System.IO;
using System.Threading.Tasks;

namespace WePrint.FileService
{
    public class BytesFile : IFile
    {
        private readonly byte[] _bytes;
        private readonly string _mimeType;

        public BytesFile(byte[] bytes, string mimeType)
        {
            _bytes = bytes;
            _mimeType = mimeType;
        }

        public byte[] AsBytes()
        {
            return _bytes;
        }

        public Stream AsStream()
        {
            return new MemoryStream(_bytes);
        }

        public async Task<byte[]> AsBytesAsync()
        {
            return _bytes;
        }

        public async Task<string> GetMimeTypeAsync()
        {
            return _mimeType;
        }

        public async Task<long> GetLength()
        {
            return _bytes.LongLength;
        }
    }
}