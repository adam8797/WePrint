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
        public async Task<ActionResult<List<SearchViewModel>>> Search([FromQuery]string q)
        {
            var projects = Database.Projects
                .Where(x => x.Title.Contains(q));

            var orgs = Database.Organizations
                .Where(x => x.Name.Contains(q) || x.Description.Contains(q));

            var pvm = await projects.Select(project => new SearchViewModel()
            {
                Description = project.Description,
                ImageUrl = Url.Action("GetThumbnail", "Project", new { id = project.Id }),
                Id = project.Id,
                Title = project.Title,
                Href = "/project/" + project.Id,
                Type = "Project"
            }).ToListAsync();

            var ovm = await orgs.Select(org => new SearchViewModel()
            {
                Title = org.Name,
                Id = org.Id,
                Description = org.Description,
                ImageUrl = Url.Action("GetOrgAvatar", "Organization", new { id = org.Id }),
                Href = "/organization/" + org.Id,
                Type = "Organization"
            }).ToListAsync();

            return pvm.Concat(ovm).OrderBy(x => x.Title).ToList();

        }
    }
}
