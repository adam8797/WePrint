using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Swashbuckle.AspNetCore.Annotations;
using WePrint.Controllers.Base;
using WePrint.Data;
using WePrint.Models;

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [SwaggerTag("Project")]
    [Route("api/projects/{parentId}/updates")]
    public class ProjectUpdatesController : WePrintRestSubController<Project, ProjectCreateModel, ProjectUpdate, ProjectUpdateViewModel, ProjectUpdateCreateModel, Guid>
    {
        public ProjectUpdatesController(IServiceProvider services) : base(services)
        {
        }

        protected override async ValueTask<ProjectUpdate> CreateDataModelAsync(Project project, ProjectUpdateCreateModel viewModel)
        {
            var update = Mapper.Map<ProjectUpdate>(viewModel);
            update.PostedBy = await CurrentUser;
            update.Project = project;
            update.Timestamp = DateTimeOffset.Now;
            return update;
        }

        protected override async ValueTask<ProjectUpdate> UpdateDataModelAsync(ProjectUpdate project, ProjectUpdateCreateModel createModel)
        {
            var update = Mapper.Map<ProjectUpdate>(createModel);
            update.PostedBy = await CurrentUser;
            update.EditTimestamp = DateTimeOffset.Now;
            return update;
        }

        protected override async ValueTask<ProjectUpdate> UpdateDataModelAsync(ProjectUpdate project, ProjectUpdateViewModel viewModel)
        {
            var update = Mapper.Map<ProjectUpdate>(viewModel);
            update.PostedBy = await CurrentUser;
            update.EditTimestamp = DateTimeOffset.Now;
            return update;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        // POST /api/[controller]
        public override async Task<ActionResult<ProjectUpdateViewModel>> Post(Guid parentId, [FromBody] ProjectUpdateCreateModel body)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var parent = await Database.FindAsync<Project>(parentId);
            if (parent == null)
                return NotFound();

            var user = await CurrentUser;
            if (user.Organization != parent.Organization) 
                return Forbid();

            var dataModel = await CreateDataModelAsync(parent, body);
            dataModel.Deleted = false;

            Database.Set<ProjectUpdate>().Add(dataModel);
            await Database.SaveChangesAsync();

            return CreatedAtAction("Get", new { parentId, id = dataModel.Id }, await CreateViewModelAsync(dataModel));
        }

        protected override IQueryable<ProjectUpdate> Filter(IQueryable<ProjectUpdate> data, Project parent, User user)
        {
            return Database.ProjectUpdates.Where(x => x.Project == parent).OrderByDescending(x => x.Timestamp);
        }
    }
}
