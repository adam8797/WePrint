using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using WePrint.Data;

namespace WePrint.FileService
{
    public class EntityFrameworkFileService : IFileService
    {
        private readonly WePrintContext _context;

        public EntityFrameworkFileService(WePrintContext context)
        {
            _context = context;
        }

        public async Task<IFile> GetFileAsync(string path)
        {
            return new EFProxyFile(_context, path);
        }

        public async Task<IDirectory> GetDirectoryAsync(string path)
        {
            return new EFProxyDirectory(_context, path);
        }

        public async Task SaveFileAsync(string path, IFile file)
        {
            var entry = new File()
            {
                Data = await file.AsBytesAsync(),
                FilePath = path,
                MimeType = await file.GetMimeTypeAsync()
            };
            _context.Files.Add(entry);
        }

        public async Task RenameFileAsync(string from, string to)
        {
            var file = _context.Files.SingleOrDefault(x => x.FilePath == from);
            if (file != null)
            {
                file.FilePath = to;
                await _context.SaveChangesAsync();
            }
        }

        public async IAsyncEnumerable<IFile> GetAllFiles()
        {
            await foreach (var f in _context.Files.Select(x => x.FilePath).AsAsyncEnumerable())
            {
                yield return new EFProxyFile(_context, f);
            }
        }
    }
}
