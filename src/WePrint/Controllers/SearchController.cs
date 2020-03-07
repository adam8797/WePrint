using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
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

            IRavenQueryable<JobModel> jobs = Database.Query<JobModel>();

            if (!string.IsNullOrWhiteSpace(q))
            {
                jobs = jobs
                    .Search(j => j.Name, q)
                    .Search(j => j.Description, q);
            }

            var result = (await jobs.ToListAsync()).Select(j => j.GetViewableJob(user?.Id));
            return Ok(result);
        }

    }
}
