using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents.Session;
using WePrint.Common.Models;
using WePrint.Common.Slicer.Interface;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SlicingTestController : ControllerBase
    {
        private readonly ILogger<SlicingTestController> _logger;
        private readonly ISlicerService _slicer;
        private readonly IAsyncDocumentSession _session;

        public SlicingTestController(ILogger<SlicingTestController> logger, ISlicerService slicer, IAsyncDocumentSession session)
        {
            _logger = logger;
            _slicer = slicer;
            _session = session;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var job = new Job();
            await _session.StoreAsync(job);

            using (var f = System.IO.File.OpenRead(@"Part1.stl"))
            {
                _session.Advanced.Attachments.Store(job, "Part1.stl", f);
                await _session.SaveChangesAsync();
            }

            await _slicer.SliceAsync(job, CancellationToken.None);

            return Ok();
        }
    }
}
