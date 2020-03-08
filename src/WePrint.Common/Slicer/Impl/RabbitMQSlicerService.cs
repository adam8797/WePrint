using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Raven.Client.Documents;
using WePrint.Common.Models;
using WePrint.Common.Slicer.Interface;
using WePrint.Common.Slicer.Models;

namespace WePrint.Common.Slicer.Impl
{
    public class RabbitMQSlicerService : ISlicerService
    {
        private readonly IBus _bus;
        private readonly IDocumentStore _store;
        private readonly string _queue;

        public RabbitMQSlicerService(IBus bus, string queue, IDocumentStore store)
        {
            _bus = bus;
            _store = store;
            _queue = queue;

        }

        public async Task<SlicerJob> SliceAsync(JobModel job, CancellationToken cancellationToken)
        {
            var slicerJob = new SlicerJob()
            {
                Files = null,
                Status = SliceJobStatus.Waiting,
                Job = job.Id,
                Id = "",
            };

            try
            {
                using (var session = _store.OpenAsyncSession())
                {
                    await session.StoreAsync(slicerJob, cancellationToken);
                    await session.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            await _bus.SendAsync(_queue, new SliceJobMessage
            {
                SliceJobId = slicerJob.Id
            });

            return slicerJob;
        }

        public async Task<SlicerJob> SliceAsync(JobModel job, IEnumerable<string> files, CancellationToken cancellationToken)
        {
            var slicerJob = new SlicerJob()
            {
                Files = files.ToArray(),
                Status = SliceJobStatus.Waiting,
                Job = job.Id,
                Id = ""
            };

            try
            {
                using (var session = _store.OpenAsyncSession())
                {
                    await session.StoreAsync(slicerJob, cancellationToken);
                    await session.SaveChangesAsync(cancellationToken);
                }
            }
            catch (Exception ex)
            {
                return null;
            }

            await _bus.SendAsync(_queue, new SliceJobMessage
            {
                SliceJobId = slicerJob.Id
            });

            return slicerJob;
        }
    }
}
