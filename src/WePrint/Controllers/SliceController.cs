using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using EasyNetQ.ConnectionString;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using WePrint.Common.Models;
using WePrint.Common.Slicer.Impl;
using WePrint.Common.Slicer.Interface;
using WePrint.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/slicing")]
    public class SliceController : WePrintController
    {
        private readonly ISlicerService _slicer;

        public SliceController(
            ILogger<JobController> logger,
            IAsyncDocumentSession database,
            UserManager<ApplicationUser> userManager,
            ISlicerService slicer)
            : base(logger, userManager, database)
        {
            _slicer = slicer;
        }


        [HttpPost]
        public async Task<IActionResult> SliceFullJob(SliceRequest model)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(model.JobId);

            SlicerJob sJob;
            if (model.Files != null && model.Files.Length > 0)
                sJob = await _slicer.SliceAsync(job, model.Files, CancellationToken.None);
            else
                sJob = await _slicer.SliceAsync(job, CancellationToken.None);

            return Ok(sJob);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> CheckSliceStatus(string id)
        {
            return Ok(await Database.LoadAsync<SlicerJob>(id));
        }

        [HttpGet("job/{id}")]
        public async Task<IActionResult> SliceStatusForJob(string id)
        {
            return Ok(await Database.Query<SlicerJob>().Where(x => x.Job == id).ToListAsync());
        }
    }
}
