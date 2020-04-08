using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace WePrint.FileService
{
    public interface IFile
    {
        byte[] AsBytes();
        
        Stream AsStream();

        Task<byte[]> AsBytesAsync();

        Task<string> GetMimeTypeAsync();

        Task<long> GetLength();
    }
}
