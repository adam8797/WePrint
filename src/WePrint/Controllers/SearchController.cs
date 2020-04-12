using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WePrint.Controllers.Base;
using WePrint.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : WePrintController
    {
        public SearchController(IServiceProvider services) : base(services)
        {
        }

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
