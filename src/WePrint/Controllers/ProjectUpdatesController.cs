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
    public class project_updates_controller : we_print_rest_sub_controller<project, project_create_model, project_update, project_update_view_model, project_update_create_model, Guid>
    {
        public project_updates_controller(IServiceProvider services) : base(services)
        {
        }

        protected override async ValueTask<project_update> create_data_model_async(project project, project_update_create_model view_model)
        {
            var update = mapper.Map<project_update>(view_model);
            update.posted_by = await current_user;
            update.project = project;
            update.timestamp = DateTimeOffset.Now;
            return update;
        }

        protected override async ValueTask<project_update> update_data_model_async(project_update project, project_update_create_model create_model)
        {
            var update = mapper.Map<project_update>(create_model);
            update.posted_by = await current_user;
            update.edit_timestamp = DateTimeOffset.Now;
            return update;
        }

        protected override async ValueTask<project_update> update_data_model_async(project_update project, project_update_view_model view_model)
        {
            var update = mapper.Map<project_update>(view_model);
            update.posted_by = await current_user;
            update.edit_timestamp = DateTimeOffset.Now;
            return update;
        }

        protected override IQueryable<project_update> filter(IQueryable<project_update> data, project parent, user user)
        {
            return database.ProjectUpdates.Where(x => x.project == parent).OrderByDescending(x => x.timestamp);
        }
    }
}
