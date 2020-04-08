using System.Collections.Generic;

namespace WePrint.FileService
{
    public interface IDirectory
    {
        IAsyncEnumerable<IFile> GetFiles();

        IAsyncEnumerable<IDirectory> GetDirectories();
    }
}