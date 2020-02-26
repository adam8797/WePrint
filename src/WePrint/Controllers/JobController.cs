using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using WePrint.Common.ServiceDiscovery;
using WePrint.Models;

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/job")]
    public class JobController : Controller
    {
        private readonly ILogger<JobController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public JobController(ILogger<JobController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }


        // Get /api/Job
        // If an Id is included as a url parameter, that job will be returned
        // Otherwise, all jobs belonging to the current user will be returned
        [HttpGet]
        public async Task<IActionResult> GetJobs([FromQuery] string Id)
        {   var user = await this.GetCurrentUser(_session);
            if (user == null)
            {
                return this.FailWith("Not Logged in", HttpStatusCode.Unauthorized);
            }

            if(Id != null) 
                return await this.QueryItemById<JobModel>(_session, Id);

            var myJobs = await _session.Query<JobModel>().Where(job => job.CustomerId == user.Id || job.Bids.Any(b => b.BidderId == user.Id)).ToArrayAsync();

            return Json(myJobs);
        }

        // POST: /api/Job/
        [HttpPost]
        public async Task<IActionResult> CreateJob()
        {
            // TODO: Get current user
            ApplicationUser currentUser = await this.GetCurrentUser(_session);
            if(currentUser == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Json(new { err = "Not logged in" });
            }

            var newJob = new JobModel()
            {
                Status = JobStatus.PendingOpen,
                Address = currentUser.Address,
                BidClose = DateTime.Today + TimeSpan.FromDays(3),
                Bids = new List<BidModel>(),
                Comments = new List<CommentModel>(),
                CustomerId = currentUser.Id,
                Description = "A new job",
                Files = new List<FileModel>(),
                IdempotencyKey = GlobalRandom.Next(),
                MaterialColor = MaterialColor.Any,
                MaterialType = MaterialType.PLA,
                Name = "New Job",
                PrinterType = PrinterType.SLA,
                Notes = "",
            };

            try
            {
                await _session.StoreAsync(newJob);
                await _session.SaveChangesAsync();
            }
            catch(Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { err = "Uncaught Exception", details = e.ToString() });
            }

            return Json(newJob.GetKey());
        }


        // PUT: /api/Job/
        [HttpPut]
        public async Task<IActionResult> UpdateJob([FromBody]JobUpdateModel update)
        {
            var jobs = await _session.Query<JobModel>().Where(job => job.Id == update.Id).ToArrayAsync();

            if (jobs.Length == 0)
                return this.FailWith("No job found with id " + update.Id, HttpStatusCode.NotFound);

            if (jobs.Length > 1)
                return this.FailWith("More than one job with id " + update.Id, HttpStatusCode.Conflict);

            var job = jobs[0];

            if (job.IdempotencyKey != update.IdempotencyKey)
                return this.FailWith(new { err = "Bad idempotency key. Job may have been updated.", jobs[0].IdempotencyKey }, HttpStatusCode.Conflict);

            var currentUser = await this.GetCurrentUser(_session);

            if (currentUser == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            if(currentUser.Id != job.CustomerId)
                return this.FailWith("Currenty user is not the customer for this job", HttpStatusCode.Unauthorized);

            if(job.Status >= JobStatus.BidSelected)
                return this.FailWith("The job already has a selected bid and cannot be updated", HttpStatusCode.Forbidden);

            job.ApplyChanges(update);
            job.IdempotencyKey = GlobalRandom.Next();

            try
            {
                await _session.StoreAsync(job);
                await _session.SaveChangesAsync();
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { err = "Uncaught Exception", details = e.ToString() });
            }

            return Json(job.GetKey());
        }

        // DELETE: /api/Job/
        [HttpDelete]
        public async Task<IActionResult> DeleteJob([FromBody] string jobId)
        {
            var jobs = await _session.Query<JobModel>().Where(job => job.Id == jobId).ToArrayAsync();

            if (jobs.Length == 0)
                return this.FailWith("No job found with id " + jobId, HttpStatusCode.NotFound);

            if (jobs.Length > 1)
                return this.FailWith("More than one job with id " + jobId, HttpStatusCode.Conflict);

            var job = jobs[0];


            var currentUser = await this.GetCurrentUser(_session);

            if (currentUser == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            if (currentUser.Id != job.CustomerId)
                return this.FailWith("Currenty user is not the customer for this job", HttpStatusCode.Unauthorized);

            if (job.Status >= JobStatus.BidSelected)
                return this.FailWith("The job already has a selected bid and cannot be updated", HttpStatusCode.Forbidden);

            try
            {
                _session.Delete(job);
                await _session.SaveChangesAsync();
            }
            catch (Exception e)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                return Json(new { err = "Uncaught Exception", details = e.ToString() });
            }

            return Json(job.GetKey());
        }
    }
}
