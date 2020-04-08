using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePrint.Data;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        public SearchController(ILogger<SearchController> log, UserManager<User> userManager, WePrintContext database) : base(log, userManager, database)
        {
        }

        // GET: /api/job/search
        /// <summary>
        /// Search for all jobs matching some string
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Job>>> SearchJob([FromQuery]string q)
        {
            IQueryable<Job> jobs = Database.Jobs;

            if (!string.IsNullOrWhiteSpace(q))
            {
                jobs = jobs.Where(x => x.Name.Contains(q) || x.Description.Contains(q));
            }

            return Ok(await jobs.ToListAsync());
        }

    }
}
