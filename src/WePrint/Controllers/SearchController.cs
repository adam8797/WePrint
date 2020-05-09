using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
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
        [AllowAnonymous]
        public async Task<ActionResult<List<SearchViewModel>>> Search([FromQuery]string q = null)
        {
            if (string.IsNullOrWhiteSpace(q))
                return new List<SearchViewModel>();

            var projects = Database.Projects
                .Where(x => EF.Functions.FreeText(x.Title, q) || EF.Functions.FreeText(x.Description, q) || EF.Functions.Like(x.Title, '%' +  q + '%'));

            var orgs = Database.Organizations
                .Where(x => EF.Functions.FreeText(x.Name, q) || EF.Functions.FreeText(x.Description, q) || EF.Functions.Like(x.Name, '%' + q + '%'));

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
