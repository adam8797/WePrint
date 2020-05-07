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
    public class SearchController : we_print_controller
    {
        public SearchController(IServiceProvider services) : base(services)
        {
        }

        /// <summary>
        /// Search for all jobs matching some string
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<List<search_view_model>>> Search([FromQuery]string q)
        {
            var projects = database.Projects
                .Where(x => EF.Functions.FreeText(x.title, q) || EF.Functions.FreeText(x.description, q));

            var orgs = database.Organizations
                .Where(x => EF.Functions.FreeText(x.name, q) || EF.Functions.FreeText(x.description, q));

            var pvm = await projects.Select(project => new search_view_model()
            {
                description = project.description,
                image_url = Url.Action("get_thumbnail", "project_controller", new { id = project.Id }),
                id = project.Id,
                title = project.title,
                href = "/project/" + project.Id,
                type = "Project"
            }).ToListAsync();

            var ovm = await orgs.Select(org => new search_view_model()
            {
                title = org.name,
                id = org.Id,
                description = org.description,
                image_url = Url.Action("get_org_avatar", "organization_controller", new { id = org.Id }),
                href = "/organization/" + org.Id,
                type = "Organization"
            }).ToListAsync();

            return pvm.Concat(ovm).OrderBy(x => x.title).ToList();

        }
    }
}
