using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WePrint.Common.Models;
using WePrint.Common.Slicer.Impl;

namespace WePrint.Common.Slicer.Interface
{
    public interface ISlicerService
    {
        Task<SlicerJob> SliceAsync(Job job, CancellationToken cancellationToken);
        Task<SlicerJob> SliceAsync(Job job, IEnumerable<string> files, CancellationToken cancellationToken);
    }
}
