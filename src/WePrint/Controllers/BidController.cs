using System;
using System.Diagnostics;
using System.Linq;
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
    [Route("api/bid")]
    public class BidController : Controller
    {
        private readonly ILogger<BidController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public BidController(ILogger<BidController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }


        // GET: /api/Bid/
        [HttpGet]
        public async Task<IActionResult> GetBids()
        {
            var user = await this.GetCurrentUser(_session);
            if (user == null)
            {
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);
            }

            var myBids = await _session.Query<BidModel>().Where(bid => bid.BidderId == user.Id).ToArrayAsync();

            return Json(myBids);
        }


        // GET: /api/Bid/
        [HttpGet("{id}")]
        public async Task<IActionResult> GetBid([FromRoute] string id)
        {
            return await this.QueryItemById<BidModel>(_session, HttpUtility.UrlDecode(id));
        }

        // POST: /api/Bid/
        [HttpPost]
        public async Task<IActionResult> CreateBid([FromBody] NewBidModel bid)
        {
            if (bid == null)
                return this.FailWith("Failed to parse bid", HttpStatusCode.BadRequest);

            var user = await this.GetCurrentUser(_session);

            if (user == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            if (user.Printers != null && user.Printers.Count == 0)
                return this.FailWith("User has no printers set up", HttpStatusCode.Conflict);

            return await EnsureJobBiddingOpen(bid.JobId, bid.JobIdempotencyKey, async job =>
            {
                BidModel newBid = new BidModel();
                ReflectionHelper.CopyPropertiesTo(bid, newBid);

                newBid.BidderId = user.Id;
                newBid.IdempotencyKey = GlobalRandom.Next();

                try
                {
                    await _session.StoreAsync(newBid); 
                    job.Bids.Add(newBid);
                    await _session.StoreAsync(job);
                    await _session.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { err = "Uncaught Exception", details = e.ToString() });
                }

                return Json(newBid.GetKey());
            });
        }

        // PUT: /api/Bid/
        [HttpPut]
        public async Task<IActionResult> UpdateBid([FromBody] BidUpdateModel update)
        {
            if (update == null)
                return this.FailWith("Failed to parse bid", HttpStatusCode.BadRequest);

            var user = await this.GetCurrentUser(_session);

            if (user == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            if (user.Printers != null && user.Printers.Count == 0)
                return this.FailWith("User has no printers set up", HttpStatusCode.Conflict);

            JobModel job;
            if (update.JobId == null)
            {
                var jobs = await _session.Query<JobModel>().Where(job => job.Bids.Any(b => b.Id == update.Id)).ToArrayAsync();

                if (jobs.Length == 0)
                    return this.FailWith("No bids found with id " + update.Id, HttpStatusCode.NotFound);

                else if(jobs.Length > 1)
                    return this.FailWith("Bid Id " + update.Id + " was ambiguous", HttpStatusCode.Conflict);

                job = jobs[0];
            }
            else
            {
                job = await _session.LoadAsync<JobModel>(update.JobId);
            }

            return await EnsureJobBiddingOpen(job, async job =>
            {
                var bid = job.Bids.Where(bid => bid.Id == update.Id && bid.BidderId == user.Id).First();
                ReflectionHelper.CopyPropertiesTo(update, bid);

                bid.IdempotencyKey = GlobalRandom.Next();

                try
                {
                    await _session.StoreAsync(bid);
                    await _session.StoreAsync(job);
                    await _session.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { err = "Uncaught Exception", details = e.ToString() });
                }

                return Json(bid.GetKey());
            });
        }
        
        // DELETE: /api/Bid/
        [HttpDelete]
        public async Task<IActionResult> DeleteBid(string id)
        {
            var user = await this.GetCurrentUser(_session);

            if (user == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            if(id == null)
                return this.FailWith("Provide an id", HttpStatusCode.BadRequest);

            var jobs = await _session.Query<JobModel>().Where(job => job.Bids.Any(b => b.Id == id && b.BidderId == user.Id)).ToArrayAsync();

            if (jobs.Length == 0)
                return this.FailWith("No bids found with id " + id, HttpStatusCode.NotFound);

            else if(jobs.Length > 1)
                return this.FailWith("Bid Id " + id + " was ambiguous", HttpStatusCode.Conflict);

            return await EnsureJobBiddingOpen(jobs[0], async job =>
            {
                job.Bids.RemoveAll(bid => bid.Id == id);

                try
                {
                    await _session.StoreAsync(job); // TODO do we want to delete the bid in it's table as well?
                    await _session.SaveChangesAsync();
                }
                catch (Exception e)
                {
                    HttpContext.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                    return Json(new { err = "Uncaught Exception", details = e.ToString() });
                }

                return Json(id);
            });
        }

        private async Task<IActionResult> EnsureJobBiddingOpen(string jobId, int jobIdempotency, Func<JobModel, Task<IActionResult>> ifJobValid)
        {
            var targetJobs = await _session.Query<JobModel>().Where(job => job.Id == jobId).ToArrayAsync();

            if (targetJobs.Length == 0)
                return this.FailWith("No job with that Id found", HttpStatusCode.NotFound);

            if (targetJobs.Length > 1)
                return this.FailWith("More than one job matched that Id. That's a problem.", HttpStatusCode.Conflict);

            var job = targetJobs[0];
            
            if(job.IdempotencyKey != jobIdempotency)
                return this.FailWith("Job has updated since this request was processed", HttpStatusCode.Conflict);

            return await EnsureJobBiddingOpen(job, ifJobValid);
        }

        private async Task<IActionResult> EnsureJobBiddingOpen(JobModel job, Func<JobModel, Task<IActionResult>> ifJobValid)
        {
            if (job.Status >= JobStatus.BiddingClosed)
                return this.FailWith("Bidding is Closed", HttpStatusCode.Conflict);

            if (job.BidClose <= DateTime.Now)
                return this.FailWith("Bidding is Closed", HttpStatusCode.Conflict);

            return await ifJobValid(job);
        }
    }
}
