using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using WePrint.Common.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : WePrintController
    {
        public SearchController(ILogger<SearchController> log, IAsyncDocumentSession database) : base(log, database)
        {
        }

        // GET: /api/job/search
        /// <summary>
        /// Search for all jobs matching some string
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<JobModel>>> SearchJob([FromQuery]string q)
        {
            var user = await CurrentUser;
            var jobs = await Database.Query<JobModel>()
                .Search(j => j.Name, q)
                .Search(j => j.Description, q)
                .ToArrayAsync();

            var returnableJobs = new List<JobModel>();
            foreach (var j in jobs)
            {
                returnableJobs.Add(j.GetViewableJob(user.Id));
            }
            return returnableJobs;
        }

    }
}
