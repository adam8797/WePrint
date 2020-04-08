using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WePrint.Data;

namespace WePrint.FileService
{
    public class EFProxyFile : IFile
    {
        private readonly WePrintContext _context;
        private readonly string _entryId;

        public EFProxyFile(WePrintContext context, string entryId)
        {
            _context = context;
            _entryId = entryId;
        }

        public byte[] AsBytes()
        {
            var bytes = _context.Files.Where(x => x.FilePath == _entryId).Select(x => x.Data).SingleOrDefault();
            return bytes;
        }

        public Stream AsStream()
        {
            return new MemoryStream(AsBytes());
        }

        public async Task<byte[]> AsBytesAsync()
        {
            return (await _context.Files.FindAsync(_entryId)).Data;
        }

        public async Task<string> GetMimeTypeAsync()
        {
            return await _context.Files.Where(x => x.FilePath == _entryId).Select(x => x.MimeType).SingleOrDefaultAsync();
        }

        public async Task<long> GetLength()
        {
            return await _context.Files.Where(x => x.FilePath == _entryId).Select(x => x.Data.LongLength).SingleOrDefaultAsync();
        }
    }
}