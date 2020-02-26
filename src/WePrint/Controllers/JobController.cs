using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using WePrint.Common.Models;
using WePrint.Common.ServiceDiscovery;

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/job")]
    public class JobController : WePrintController
    {
        private readonly IConfiguration _configuration;

        public JobController(ILogger<JobController> logger, IAsyncDocumentSession database, IConfiguration configuration) : base(logger, database)
        {
            _configuration = configuration;
        }

        #region Base Job Methods

        // Get /api/job
        /// <summary>
        /// Gets all jobs that the current user is a part of
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobModel>>> GetJobs()
        {
            var user = await CurrentUser;
            return await Database.Query<JobModel>().Where(job => job.CustomerId == user.Id || job.Bids.Any(x => x.BidderId == user.Id)).ToArrayAsync();
        }

        // GET: /api/job/{id}
        /// <summary>
        /// Get a particular job by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<JobModel>> GetJob(string id)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if (await CheckJobAccess(job))
                return job;

            return Forbid();
        }

        // POST: /api/job/
        /// <summary>
        /// Create a new Job with defaults
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<JobModel>> CreateJob() //ToDo: This should have a post model
        {
            var user = await CurrentUser;

            var newJob = new JobModel()
            {
                Status = JobStatus.PendingOpen,
                Address = user.Address,
                BidClose = DateTime.Today + TimeSpan.FromDays(3),
                Bids = new List<BidModel>(),
                Comments = new List<CommentModel>(),
                CustomerId = user.Id,
                Description = "A new job",
                IdempotencyKey = GlobalRandom.Next(),
                MaterialColor = MaterialColor.Any,
                MaterialType = MaterialType.PLA,
                Name = "New Job",
                PrinterType = PrinterType.SLA,
                Notes = "",
            };

            await Database.StoreAsync(newJob);
            await Database.SaveChangesAsync();

            return newJob;
        }

        // PUT: /api/job/{id}
        /// <summary>
        /// Create or update a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<JobModel>> UpdateJob(string id, [FromBody] JobUpdateModel update)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
            {
                var j = new JobModel();
                j.ApplyChanges(update);
                await Database.StoreAsync(j);
            }
            else
            {
                if (job.IdempotencyKey != update.IdempotencyKey)
                    return StatusCode((int) HttpStatusCode.Conflict, "Bad idempotency key. Job may have been updated.");

                if ((await CurrentUser).Id != job.CustomerId)
                    return Unauthorized("Currently user is not the customer for this job");

                if (job.Status >= JobStatus.BidSelected)
                    return Forbid();

                job.ApplyChanges(update);
                job.IdempotencyKey = GlobalRandom.Next();
            }
            
            await Database.SaveChangesAsync();
            return job;
        }

        /// <summary>
        /// Apply a JSON Patch to a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{id}")]
        public async Task<ActionResult<JobModel>> PatchJob(string id, [FromBody] JsonPatchDocument<JobModel> patchDoc)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            patchDoc.ApplyTo(job);

            await Database.SaveChangesAsync();

            return job;
        }


        // DELETE: /api/job/{id}
        /// <summary>
        /// Delete a job
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteJob(string id)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if ((await CurrentUser).Id != job.CustomerId)
                return Forbid();

            if (job.Status >= JobStatus.BidSelected)
                return Forbid();

            Database.Delete(job);
            await Database.SaveChangesAsync();

            return Ok();
        }

        #endregion

        #region Files

        // GET: /api/job/{id}/files
        /// <summary>
        /// List files that are attached to a given job
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/files")]
        public async Task<IActionResult> GetFiles(string id)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            var files = Database.Advanced.Attachments.GetNames(job);

            return Json(files.Select(x => new
            {
                x.Name,
                x.Size
            }));
        }

        // GET: /api/job/{id}/files/{filename}
        /// <summary>
        /// Download an attached file from a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpGet("{id}/files/{filename}")]
        public async Task<IActionResult> GetFile(string id, string filename)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            var attachment = await Database.Advanced.Attachments.GetAsync(job, filename);

            if (attachment == null)
                return NotFound(filename);

            return File(attachment.Stream, attachment.Details.ContentType, filename);
        }

        // POST: /api/job/{id}/files
        /// <summary>
        /// Upload a single file to a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="file"></param>
        /// <returns></returns>
        [HttpPost("{id}/files")]
        public async Task<IActionResult> UploadFile(string id, IFormFile file)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            if (file.Length <= 0)
                return BadRequest("File Length <= 0");

            var maxSizeInMegs = _configuration.GetValue("FileUploads:MaxSizeInMegabytes", 100.0);
            var maxSizeInBytes = (int)(maxSizeInMegs * 1_000_000);

            if (file.Length >= maxSizeInBytes)
                return BadRequest($"File too large. Max size is {maxSizeInBytes} bytes");


            var allowedExtensions = _configuration.GetSection("FileUploads:AllowedExtensions").Get<string[]>();
            if (allowedExtensions != null && !Path.GetExtension(file.FileName).In(allowedExtensions))
                return BadRequest($"File Extension must be one of: {string.Join(", ", allowedExtensions)}");

            Database.Advanced.Attachments.Store(job, file.FileName, file.OpenReadStream(), file.ContentType);
            await Database.SaveChangesAsync();

            //ToDO: Maybe return something more than OK here.
            return NoContent();
        }

        // DELETE: /api/job/{id}/files/{filename}
        /// <summary>
        /// Delete an attached file from a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpDelete("{id}/files/{filename}")]
        public async Task<IActionResult> DeleteFile(string id, string filename)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            Database.Advanced.Attachments.Delete(job, filename);

            await Database.SaveChangesAsync();

            return NoContent();
        }

        // PATH: /api/job/{id}/files/{filename}/rename/{newName}
        /// <summary>
        /// Rename an attached file
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <param name="newName"></param>
        /// <returns></returns>
        [HttpPatch("{id}/files/{filename}/rename/{newName}")]
        public async Task<IActionResult> RenameFile(string id, string filename, string newName)
        {
            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            Database.Advanced.Attachments.Rename(job, filename, newName);

            await Database.SaveChangesAsync();

            return NoContent();
        }

        #endregion

        private async Task<bool> CheckJobAccess(JobModel job)
        {
            try
            {
                var user = await CurrentUser;

                if (user == null)
                    return false;

                return job.CustomerId == user.Id || job.Bids.Any(x => x.BidderId == user.Id);
            }
            catch
            {
                return false;
            }
        }
    }
}
