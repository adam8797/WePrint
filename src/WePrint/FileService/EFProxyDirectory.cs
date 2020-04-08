using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using WePrint.Data;

namespace WePrint.FileService
{
    public class EFProxyDirectory : IDirectory
    {
        private readonly WePrintContext _context;
        private readonly string _directory;

        public EFProxyDirectory(WePrintContext context, string directory)
        {
            _context = context;
            _directory = directory;
        }

        public async IAsyncEnumerable<IFile> GetFiles()
        {
            var potential = await _context.Files.Select(x => x.FilePath).Where(x => x.StartsWith(_directory)).ToListAsync();
            foreach (var file in potential)
            {
                if (Path.GetDirectoryName(file).Replace('\\', '/') == _directory)
                    yield return new EFProxyFile(_context, file);
            }
        }

        public async IAsyncEnumerable<IDirectory> GetDirectories()
        {
            var potential = await _context.Files.Select(x => x.FilePath).Where(x => x.StartsWith(_directory)).ToListAsync();
            var dirs = potential.Select(x => x.Remove(0, _directory.Length).Split('/')).Select(x => x.FirstOrDefault());
            foreach (var file in dirs)
            {
                yield return new EFProxyDirectory(_context, Path.Combine(_directory, file));
            }
        }
    }
}