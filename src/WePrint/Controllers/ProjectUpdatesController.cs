using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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

        protected override IQueryable<ProjectUpdate> Filter(IQueryable<ProjectUpdate> data, Project parent, User user)
        {
            return Database.ProjectUpdates.Where(x => x.Project == parent).OrderByDescending(x => x.Timestamp);
        }
    }
}
