using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePrint.Data;
using WePrint.ViewModels;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        public SearchController(IServiceProvider services) : base(services)
        {
        }

        // GET: /api/job/search
        /// <summary>
        /// Search for all jobs matching some string
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<List<JobViewModel>>> SearchJob([FromQuery]string q)
        {
            IQueryable<Job> jobs = Database.Jobs;

            if (!string.IsNullOrWhiteSpace(q))
            {
                jobs = jobs.Where(x => x.Name.Contains(q) || x.Description.Contains(q));
            }

            return await jobs.ProjectTo<JobViewModel>(Mapper.ConfigurationProvider).ToListAsync();
        }

    }
}
