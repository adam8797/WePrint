using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
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
    [ApiController]
    [Route("api/job")]
    public class JobController : WePrintController
    {
        private readonly IConfiguration _configuration;

        public JobController(
            ILogger<JobController> logger, 
            IAsyncDocumentSession database, 
            UserManager<ApplicationUser> userManager, 
            IConfiguration configuration) 
            : base(logger, userManager, database)
        {
            _configuration = configuration;
        }

        #region Base Job Methods

        // Get /api/job
        /// <summary>
        /// Gets all jobs that the current user is a part of
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobViewModel>>> GetJobs()
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var jobs = await Database.Query<JobModel>()
                .Where(job => job.CustomerId == user.Id || job.Bids.Any(x => x.BidderId == user.Id))
                .ToArrayAsync();

            var allUsers = await GetUsers(); // This is a kludge because I can't figure out how to include all the fields we need
            return jobs.Select(job => new JobViewModel(job, allUsers, user.Id)).ToList();

        }


        // GET: /api/job/{id}
        /// <summary>
        /// Get a particular job by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<JobViewModel>> GetJob(string id)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            var allUsers = await GetUsers();
            if (await CheckJobAccess(job))
                return new JobViewModel(job, allUsers, user.Id);

            return Forbid();
        }

        // POST: /api/job/
        /// <summary>
        /// Create a new Job with defaults
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<JobModel>> CreateJob([FromBody] JobUpdateModel enteredJob)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var newJob = new JobModel()
            {
                Id = "",
                Status = JobStatus.PendingOpen,
                Address = user.Address,
                BidClose = enteredJob.BidClose ?? DateTime.Today + TimeSpan.FromDays(3),
                Bids = new List<BidModel>(),
                Comments = new List<CommentModel>(),
                CustomerId = user.Id,
                Description = enteredJob.Description,
                MaterialColor = enteredJob.MaterialColor ?? MaterialColor.Any,
                MaterialType = enteredJob.MaterialType ?? MaterialType.PLA,
                Name = enteredJob.Name,
                PrinterType = enteredJob.PrinterType ?? PrinterType.SLA,
                Notes = enteredJob.Notes,
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
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
            {
                var j = new JobModel();
                j.ApplyChanges(update);
                await Database.StoreAsync(j);
            }
            else
            {
                if ((await CurrentUser).Id != job.CustomerId)
                    return Unauthorized("Currently user is not the customer for this job");

                if (job.Status >= JobStatus.BidSelected)
                    return Forbid();

                job.ApplyChanges(update);
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
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

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
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

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
        [SwaggerOperation(Tags = new []{ "Files" })]
        public async Task<IActionResult> GetFiles(string id)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

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
        [SwaggerOperation(Tags = new []{ "Files" })]
        public async Task<IActionResult> GetFile(string id, string filename)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

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
        [SwaggerOperation(Tags = new []{ "Files" })]
        public async Task<IActionResult> UploadFile(string id, IFormFile file)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

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

            return Created(Url.Action("GetFile", new {id, filename = file.FileName}),
                new {Name = file.FileName, Size = file.Length});
        }

        // DELETE: /api/job/{id}/files/{filename}
        /// <summary>
        /// Delete an attached file from a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="filename"></param>
        /// <returns></returns>
        [HttpDelete("{id}/files/{filename}")]
        [SwaggerOperation(Tags = new []{ "Files" })]
        public async Task<IActionResult> DeleteFile(string id, string filename)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

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
        [SwaggerOperation(Tags = new []{ "Files" })]
        public async Task<IActionResult> RenameFile(string id, string filename, string newName)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

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

        #region Comments


        // GET: /api/job/{id}/comments
        /// <summary>
        /// Get a list of all comments
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/comments")]
        [SwaggerOperation(Tags = new []{ "Comments" })]
        public async Task<ActionResult<List<CommentViewModel>>> ListComments(string id)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            var allUsers = await GetUsers(); // Probably easy to put an includes here
            return job.Comments.Select(c => new CommentViewModel(c, allUsers)).ToList();
        }

        // GET: /api/job/{id}/comments/{commentId}
        /// <summary>
        /// Get a particular comment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpGet("{id}/comments/{commentId}")]
        [SwaggerOperation(Tags = new []{ "Comments" })]
        public async Task<ActionResult<CommentViewModel>> GetComment(string id, int commentId)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            var comment = job.Comments.SingleOrDefault(x => x.Id == commentId);

            if (comment == null)
                return NotFound();


            var allUsers = await GetUsers();
            return new CommentViewModel(comment, allUsers);
        }

        // POST: /api/job/{id}/comments
        /// <summary>
        /// Add a new comment to a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="comment"></param>
        /// <returns></returns>
        [HttpPost("{id}/comments")]
        [SwaggerOperation(Tags = new []{ "Comments" })]
        public async Task<IActionResult> AddComment(string id, CommentModel comment)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            comment.CommenterId = (await CurrentUser).Id;
            comment.Timestamp = DateTime.Now;
            comment.Id = job.Comments.Select(x => x.Id).Prepend(0).Max() + 1;
            comment.Edited = false;

            job.Comments.Add(comment);

            await Database.SaveChangesAsync();

            return Created(Url.Action("GetComment", new { id, commentId = comment.Id }), comment);

        }

        // PUT: /api/job/{id}/comments/{commentId}
        /// <summary>
        /// Add or Update a comment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commentId"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPut("{id}/comments/{commentId}")]
        [SwaggerOperation(Tags = new []{ "Comments" })]
        public async Task<IActionResult> AddOrUpdateComment(string id, int commentId, CommentModel update)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            if (job == null)
                return NotFound("Job not found");

            update.CommenterId = (await CurrentUser).Id;
            update.Timestamp = DateTime.Now;
            update.Edited = false;

            var existing = job.Comments.SingleOrDefault(x => x.Id == commentId);

            if (existing != null)
            {
                // Update the comment
                if (existing.CommenterId != (await CurrentUser).Id)
                    return Forbid();

                existing.Text = update.Text;
                existing.Edited = true;
            }
            else
            {
                // Create the comment
                update.Id = job.Comments.Select(x => x.Id).Prepend(0).Max() + 1;
                job.Comments.Add(update);
            }

            await Database.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /api/job/{id}/comments/{commentId}
        /// <summary>
        /// Delete a comment
        /// </summary>
        /// <param name="id"></param>
        /// <param name="commentId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/comments/{commentId}")]
        [SwaggerOperation(Tags = new []{ "Comments" })]
        public async Task<ActionResult<CommentModel>> DeleteComment(string id, int commentId)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            if (job == null)
                return NotFound("Job Not found");

            var comment = job.Comments.SingleOrDefault(x => x.Id == commentId);

            if (comment == null)
                return NotFound();

            if (comment.CommenterId != (await CurrentUser).Id)
                return Forbid();

            job.Comments.Remove(comment);

            await Database.SaveChangesAsync();

            return Ok();
        }


        #endregion
        
        #region Bids

        // GET : /api/job/{id}/bids
        /// <summary>
        /// Get a list of all bids associated with the job
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}/bids")] 
        [SwaggerOperation(Tags = new []{ "Bids" })]
        public async Task<ActionResult<List<BidViewModel>>> ListBids(string id)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);


            if (job.CustomerId != user.Id && job.Status < JobStatus.BiddingClosed)
                return Forbid();


            var allUsers = await GetUsers();
            return job.Bids.Select(b => new BidViewModel(b, allUsers)).ToList();
        }

        // GET : /api/job/{id}/bids/{bidId}
        /// <summary>
        /// Get a particular bid from a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bidId"></param>
        /// <returns></returns>
        [HttpGet("{id}/bids/{bidId}")] 
        [SwaggerOperation(Tags = new []{ "Bids" })]
        public async Task<ActionResult<BidViewModel>> GetBid(string id, int bidId)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);

            var bid = job.Bids.SingleOrDefault(x => x.Id == bidId);
            if (bid == null)
                return NotFound();

            if (job.CustomerId != user.Id && bid.BidderId != user.Id && job.Status < JobStatus.BiddingClosed)
                return Forbid();

            var allUsers = await GetUsers();
            return new BidViewModel(bid, allUsers);
        }

        // POST: /api/job/{id}/bids
        /// <summary>
        /// Add a new comment to a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bid"></param>
        /// <returns></returns>
        [HttpPost("{id}/bids")]
        [SwaggerOperation(Tags = new []{ "Bids" })]
        public async Task<IActionResult> AddBid(string id, BidModel bid)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound(id);


            if (user.Printers == null || !user.Printers.Any())
                return Forbid(); // Is this the right return code? Feels good enough

            bid.BidderId = user.Id;
            bid.Id = job.Bids.Select(x => x.Id).Prepend(0).Max() + 1;

            job.Bids.Add(bid);

            await Database.SaveChangesAsync();

            return Created(Url.Action("GetBid", new { id, bidId = bid.Id }), bid);

        }

        // PUT: /api/job/{id}/bids/{bidId}
        /// <summary>
        /// Add or Update a bid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bidId"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPut("{id}/bids/{bidId}")]
        [SwaggerOperation(Tags = new []{ "Bids" })]
        public async Task<IActionResult> AddOrUpdateBid(string id, int bidId, BidModel update)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);
            if (job == null)
                return NotFound("Job not found");

            update.BidderId = (await CurrentUser).Id;
            var oldBid = job.Bids.SingleOrDefault(x => x.Id == bidId);

            if (oldBid == null)
            {
                update.Id = job.Bids.Select(x => x.Id).Prepend(0).Max() + 1;
                job.Bids.Add(update);
            }
            else
            {
                if (oldBid.BidderId != user.Id)
                    return Forbid();

                oldBid.ApplyChanges(update);
            }

            await Database.SaveChangesAsync();

            return NoContent();
        }

        /// <summary>
        /// Apply a JSON Patch to a bid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bidId"></param>
        /// <param name="patchDoc"></param>
        /// <returns></returns>
        [HttpPatch("{id}/bids/{bidId}")]
        [SwaggerOperation(Tags = new []{ "Bids" })]
        public async Task<ActionResult<BidModel>> PatchBid(string id, int bidId, [FromBody] JsonPatchDocument<BidModel> patchDoc)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);
            if (job == null)
                return NotFound("Job not found");

            var bid = job.Bids.SingleOrDefault(x => x.Id == bidId);
            if (bid == null)
                return NotFound("Bid not found");

            if (bid.BidderId != (await CurrentUser).Id)
                return Forbid();

            patchDoc.ApplyTo(bid);

            await Database.SaveChangesAsync();

            return bid;
        }

        // DELETE: /api/job/{id}/bids/{bidId}
        /// <summary>
        /// Delete a bid
        /// </summary>
        /// <param name="id"></param>
        /// <param name="bidId"></param>
        /// <returns></returns>
        [HttpDelete("{id}/bids/{bidId}")]
        [SwaggerOperation(Tags = new []{ "Bids" })]
        public async Task<ActionResult<BidModel>> DeleteBid(string id, int bidId)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.LoadAsync<JobModel>(id);

            if (job == null)
                return NotFound("Job Not found");

            var bid = job.Bids.SingleOrDefault(x => x.Id == bidId);

            if (bid == null)
                return NotFound();

            if (bid.BidderId != (await CurrentUser).Id)
                return Forbid();

            job.Bids.Remove(bid);

            await Database.SaveChangesAsync();

            return Ok();
        }
        
        #endregion


        // I think that job access ends up being pretty situational, a lot of stuff gets opened up once bidding is closed
        // we will probably need to write custom logic at least in some places
        private async Task<bool> CheckJobAccess(JobModel job)
        {
            try
            {
                var user = await CurrentUser;

                if (user == null)
                    return false;

                return job.CustomerId == user.Id || job.Bids.Any(x => x.BidderId == user.Id || job.Status > JobStatus.PendingOpen);
            }
            catch
            {
                return false;
            }
        }
    }
}
