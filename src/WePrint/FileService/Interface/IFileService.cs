using System.Collections.Generic;
using System.Threading.Tasks;

namespace WePrint.FileService
{
    public interface IFileService
    {
        Task<IFile> GetFileAsync(string path);
        Task<IDirectory> GetDirectoryAsync(string path);
        Task SaveFileAsync(string path, IFile file);
        Task RenameFileAsync(string @from, string to);
        IAsyncEnumerable<IFile> GetAllFiles();
    }
}