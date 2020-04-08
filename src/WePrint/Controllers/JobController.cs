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
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using WePrint.Data;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/job")]
    [Authorize]
    public class JobController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public JobController(
            ILogger<JobController> logger, 
            WePrintContext database, 
            UserManager<User> userManager, 
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
        public async Task<ActionResult<IEnumerable<Job>>> GetJobs()
        {
            var user = await CurrentUser;

            var jobs = await Database.Jobs
                .Where(job => job.Customer == user || job.Bids.Any(x => x.Bidder == user) || job.Status > JobStatus.PendingOpen)
                .ToArrayAsync();

            return jobs;
        }


        // GET: /api/job/{id}
        /// <summary>
        /// Get a particular job by its ID
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<Job>> GetJob(Guid id)
        {
            var job = await Database.Jobs.FindAsync(id);

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
        public async Task<ActionResult<Job>> CreateJob([FromBody] Job enteredJob)
        {
            enteredJob.Customer = await CurrentUser;
            
            Database.Jobs.Add(enteredJob);
            await Database.SaveChangesAsync();

            return CreatedAtAction("GetJob", new {id = enteredJob.Id}, enteredJob);
        }

        // PUT: /api/job/{id}
        /// <summary>
        /// Create or update a job
        /// </summary>
        /// <param name="id"></param>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<Job>> UpdateJob(Guid id, [FromBody] Job update)
        {
            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
            {
                Database.Jobs.Add(update);
            }
            else
            {
                if (await CurrentUser != job.Customer)
                    return Unauthorized("Currently user is not the customer for this job");

                if (job.Status >= JobStatus.BidSelected)
                    return Forbid();

                Database.Jobs.Update(update);
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
        public async Task<ActionResult<Job>> PatchJob(Guid id, [FromBody] JsonPatchDocument<Job> patchDoc)
        {
            var job = await Database.Jobs.FindAsync(id);

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
        public async Task<IActionResult> DeleteJob(Guid id)
        {
            var job = await Database.Jobs.FindAsync(id);

            if (await CurrentUser != job.Customer)
                return Forbid();

            if (job.Status >= JobStatus.BidSelected)
                return Forbid();

            Database.Jobs.Remove(job);
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
        public async Task<IActionResult> GetFiles(Guid id)
        {
            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound(id);

            return Ok(job.Attachments.Select(x => new {Name = x.FileName, x.Data.Length}).ToList());
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
        public async Task<IActionResult> GetFile(Guid id, string filename)
        {
            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            var attachment = job.Attachments.SingleOrDefault(x => x.FileName == filename);

            if (attachment == null)
                return NotFound(filename);

            return File(attachment.Data, attachment.MimeType, filename);
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
        [RequestSizeLimit(100 * 1_000_000)]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var job = await Database.Jobs.FindAsync(id);

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
            if (allowedExtensions != null && !allowedExtensions.Contains(Path.GetExtension(file.FileName)))
                return BadRequest($"File Extension must be one of: {string.Join(", ", allowedExtensions)}");

            await using var ms = new MemoryStream();
            await file.OpenReadStream().CopyToAsync(ms);

            job.Attachments.Add(new Attachment()
            {
                FileName = file.FileName,
                MimeType = file.ContentType,
                Data = ms.ToArray()
            });
            await Database.SaveChangesAsync();

            return CreatedAtAction("GetFile", new {id, filename = file.FileName}, new {Name = file.FileName, Size = file.Length});
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
        public async Task<IActionResult> DeleteFile(Guid id, string filename)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync(id);
            if (job == null)
                return NotFound(id);

            var attachment = job.Attachments.SingleOrDefault(x => x.FileName == filename);
            if (attachment == null)
                return NotFound();


            if (!await CheckJobAccess(job))
                return Forbid();

            job.Attachments.Remove(attachment);

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
        public async Task<IActionResult> RenameFile(Guid id, string filename, string newName)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound(id);

            var attachment = job.Attachments.SingleOrDefault(x => x.FileName == filename);
            if (attachment == null)
                return NotFound();

            if (!await CheckJobAccess(job))
                return Forbid();

            if (job.Attachments.Any(x => x.FileName == newName))
                return BadRequest();

            attachment.FileName = newName;

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
        public async Task<ActionResult<IList<Comment>>> ListComments(Guid id)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            return Ok(job.Comments);
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
        public async Task<ActionResult<Comment>> GetComment(Guid id, Guid commentId)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync();

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            var comment = job.Comments.SingleOrDefault(x => x.Id == commentId);

            if (comment == null)
                return NotFound();

            return comment;
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
        public async Task<IActionResult> AddComment(Guid id, Comment comment)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound(id);

            if (!await CheckJobAccess(job))
                return Forbid();


            comment.Commenter = await CurrentUser;
            comment.Timestamp = DateTime.Now;
            comment.Edited = false;

            job.Comments.Add(comment);

            await Database.SaveChangesAsync();

            return CreatedAtAction("GetComment", new { id, commentId = comment.Id }, comment);

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
        public async Task<IActionResult> AddOrUpdateComment(Guid id, Guid commentId, Comment update)
        {
            var job = await Database.Jobs.FindAsync(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            if (job == null)
                return NotFound("Job not found");

            update.Commenter = await CurrentUser;
            update.Timestamp = DateTime.Now;
            update.Edited = false;

            var existing = job.Comments.SingleOrDefault(x => x.Id == commentId);

            if (existing != null)
            {
                // Update the comment
                if (existing.Commenter != await CurrentUser)
                    return Forbid();

                existing.Text = update.Text;
                existing.Edited = true;
            }
            else
            {
                // Create the comment

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
        public async Task<ActionResult<Comment>> DeleteComment(Guid id, Guid commentId)
        {
            var job = await Database.Jobs.FindAsync(id);

            if (!await CheckJobAccess(job))
                return Forbid();

            if (job == null)
                return NotFound("Job Not found");

            var comment = job.Comments.SingleOrDefault(x => x.Id == commentId);

            if (comment == null)
                return NotFound();

            if (comment.Commenter != await CurrentUser)
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
        public async Task<ActionResult<IList<Bid>>> ListBids(Guid id)
        {
            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound(id);

            if (job.Customer != await CurrentUser && job.Status < JobStatus.BiddingClosed)
                return Forbid();

            return Ok(job.Bids);
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
        public async Task<ActionResult<Bid>> GetBid(Guid id, Guid bidId)
        {
            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound(id);

            var bid = job.Bids.SingleOrDefault(x => x.Id == bidId);
            if (bid == null)
                return NotFound();

            if (job.Customer != await CurrentUser && bid.Bidder != await CurrentUser && job.Status < JobStatus.BiddingClosed)
                return Forbid();

            return bid;
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
        public async Task<IActionResult> AddBid(Guid id, Bid bid)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound(id);

            if (user.Printers == null || !user.Printers.Any())
                return Forbid();

            bid.Bidder = user;
            job.Bids.Add(bid);

            await Database.SaveChangesAsync();

            return CreatedAtAction("GetBid", new { id, bidId = bid.Id }, bid);

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
        public async Task<IActionResult> AddOrUpdateBid(Guid id, Guid bidId, Bid update)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found");

            update.Bidder = await CurrentUser;
            var oldBid = job.Bids.SingleOrDefault(x => x.Id == bidId);

            if (oldBid == null)
            {
                job.Bids.Add(update);
            }
            else
            {
                update.Id = oldBid.Id;
                Database.Update(update);
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
        public async Task<ActionResult<Bid>> PatchBid(Guid id, Guid bidId, [FromBody] JsonPatchDocument<Bid> patchDoc)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync(id);
            if (job == null)
                return NotFound("Job not found");

            var bid = job.Bids.SingleOrDefault(x => x.Id == bidId);
            if (bid == null)
                return NotFound("Bid not found");

            if (bid.Bidder != await CurrentUser)
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
        public async Task<ActionResult<Bid>> DeleteBid(Guid id, Guid bidId)
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            var job = await Database.Jobs.FindAsync(id);

            if (job == null)
                return NotFound("Job Not found");

            var bid = job.Bids.SingleOrDefault(x => x.Id == bidId);

            if (bid == null)
                return NotFound();

            if (bid.Bidder != await CurrentUser)
                return Forbid();

            job.Bids.Remove(bid);

            await Database.SaveChangesAsync();

            return Ok();
        }
        
        #endregion


        // I think that job access ends up being pretty situational, a lot of stuff gets opened up once bidding is closed
        // we will probably need to write custom logic at least in some places
        private async Task<bool> CheckJobAccess(Job job)
        {
            try
            {
                var user = await CurrentUser;

                if (user == null)
                    return false;

                return job.Customer == user || job.Bids.Any(x => x.Bidder == user) || job.Status > JobStatus.PendingOpen;
            }
            catch
            {
                return false;
            }
        }
    }
}
