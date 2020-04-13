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
        public async Task<ActionResult<List<SearchViewModel>>> SearchJob([FromQuery]string q)
        {
            return await Database.Projects.Where(x => x.Title.Contains(q)).ProjectTo<SearchViewModel>(Mapper.ConfigurationProvider).ToListAsync();
        }
    }
}
