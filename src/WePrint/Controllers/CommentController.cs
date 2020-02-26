using System;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Net;
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
    [Route("api/comment")]
    public class CommentController : Controller
    {
        private readonly ILogger<CommentController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public CommentController(ILogger<CommentController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }


        // GET" /api/Comment/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetCommentByID([FromRoute]string id)
        {
            var user = await this.GetCurrentUser(_session);
            if (user == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            if (id != null)
            {
                var jobs = await _session.Query<JobModel>().Where(job => job.Id == id).ToArrayAsync();
                if (jobs.Length == 0)
                    return this.FailWith("No jobs found with id " + id, HttpStatusCode.NotFound);

                else if(jobs.Length > 1)
                    return this.FailWith("Job Id " + id + " was ambiguous", HttpStatusCode.Conflict);
                
                var job = jobs[0];
                if (job.CustomerId == user.Id || job.MakerId == user.Id)
                {
                    return Json(job.Comments);
                }
                else 
                {
                    return this.FailWith("User has no relation to job " + id, HttpStatusCode.Unauthorized);
                }
            }

            return this.FailWith("No ID provided", HttpStatusCode.ExpectationFailed);
        }

        // POST: /api/Comment/
        [HttpPost]
        public async Task<IActionResult> CreateComment([FromBody]CommentUpdateModel create)
        {
            ApplicationUser user = await this.GetCurrentUser(_session);
            if(user == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Json(new { err = "Not logged in" });
            }

            if (create.JobId != null)
            {
                var jobs = await _session.Query<JobModel>().Where(job => job.Id == create.JobId).ToArrayAsync();
                if (jobs.Length == 0)
                    return this.FailWith("No jobs found with id " + create.JobId, HttpStatusCode.NotFound);

                else if(jobs.Length > 1)
                    return this.FailWith("Job Id " + create.JobId + " was ambiguous", HttpStatusCode.Conflict);
                
                var job = jobs[0];
                if (job.CustomerId == user.Id || job.MakerId == user.Id)
                {
                    var comment = new CommentModel()
                    {      
                        Text = create.Text,
                        CommenterId = user.Id,
                        Time = DateTime.Now
                    };

                    job.Comments.Add(comment);
                    try 
                    {
                        await _session.StoreAsync(comment);
                        await _session.StoreAsync(job);
                        await _session.SaveChangesAsync();
                    }
                    catch(Exception e)
                    {
                        return this.FailWith(new { err = "Uncaught Exception", details = e.ToString()}, HttpStatusCode.InternalServerError);
                    }

                    return Json(comment.Id);
                }
                else 
                {
                    return this.FailWith("User has no relation to job " + create.JobId, HttpStatusCode.Unauthorized);
                }
            }

            return this.FailWith("No Job ID provided", HttpStatusCode.ExpectationFailed);
        }

        // PUT: /api/Comment/
        [HttpPut]
        public async Task<IActionResult> UpdateComment([FromBody]CommentUpdateModel update)
        {
            ApplicationUser user = await this.GetCurrentUser(_session);
            if(user == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Json(new { err = "Not logged in" });
            }

            if (update.JobId != null)
            {
                var jobs = await _session.Query<JobModel>().Where(job => job.Id == update.JobId).ToArrayAsync();
                if (jobs.Length == 0)
                    return this.FailWith("No jobs found with id " + update.JobId, HttpStatusCode.NotFound);

                else if(jobs.Length > 1)
                    return this.FailWith("Job Id " + update.JobId + " was ambiguous", HttpStatusCode.Conflict);
                
                var job = jobs[0];
                var comments = job.Comments.Where(comment => comment.Id == update.Id && comment.CommenterId == user.Id);
                
                if (comments.Count() == 0)
                    return this.FailWith("No comment with id " + update.Id, HttpStatusCode.NotFound);

                if (comments.Count() > 1)
                    return this.FailWith("More than one comment with ID " + update.Id, HttpStatusCode.Conflict);
                
                var comment = comments.First();

                ReflectionHelper.CopyPropertiesTo(update, comment);
                
                try 
                {
                    await _session.StoreAsync(comment);
                    await _session.StoreAsync(job);
                    await _session.SaveChangesAsync();
                }
                catch(Exception e)
                {
                    return this.FailWith(new { err = "Uncaught Exception", details = e.ToString()}, HttpStatusCode.InternalServerError);
                }

                return Json(comment.Id);
            }

            return this.FailWith("No Job Id provided ", HttpStatusCode.ExpectationFailed);
        }

        // DELETE: /api/Comment/
        [HttpDelete]
        public async Task<IActionResult> DeleteComment([FromBody]string commentId)
        {
            ApplicationUser user = await this.GetCurrentUser(_session);
            if(user == null)
            {
                HttpContext.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                return Json(new { err = "Not logged in" });
            }

            var jobs = await _session.Query<JobModel>().Where(job => job.Comments.Any(c => c.Id == commentId && c.CommenterId == user.Id)).ToArrayAsync();

            var comments = await _session.Query<CommentModel>().Where(c => c.Id == commentId).ToArrayAsync();

            if (jobs.Length == 0)
                return this.FailWith("No comments found with id " + commentId, HttpStatusCode.NotFound);

            else if(jobs.Length > 1)
                return this.FailWith("Comment Id " + commentId + " was ambiguous", HttpStatusCode.Conflict);
            

            var job = jobs[0];
            job.Comments.RemoveAll(comment => comment.Id == commentId);
            try 
            {
                await _session.StoreAsync(job);
                await _session.SaveChangesAsync();
            }
            catch(Exception e)
            {
                return this.FailWith(new { err = "Uncaught Exception", details = e.ToString()}, HttpStatusCode.InternalServerError);
            }

            return Json(commentId);
        }
    }
}
