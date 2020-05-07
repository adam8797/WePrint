using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using WePrint.Controllers.Base;
using WePrint.Models;

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [SwaggerTag("Project")]
    [Route("api/projects/{parentId}/pledges")]
    public class project_pledges_controller : we_print_rest_sub_controller<project, project_create_model, pledge, pledge_view_model, pledge_create_model, Guid>
    {
        public project_pledges_controller(IServiceProvider services) : base(services)
        {
        }

        protected override async ValueTask<pledge> create_data_model_async(project parent, pledge_create_model view_model)
        {
            var pledge = mapper.Map<pledge>(view_model);
            pledge.maker = await current_user;
            pledge.project = parent;
            pledge.status = PledgeStatus.NotStarted;
            pledge.created = DateTimeOffset.Now;
            return pledge;
        }

        protected override IQueryable<pledge> filter(IQueryable<pledge> data, project parent, user user)
        {
            return database.Pledges.Where(x => x.project == parent);
        }

        [HttpPatch("{id}/setstatus")]
        public async Task<IActionResult> update_status(Guid parent_id, Guid id, PledgeStatus new_status)
        {
            var pledge = await database.Pledges.FindAsync(id);

            if (pledge.project.Id != parent_id)
                return NotFound();

            var user = await current_user;
            if (!((user == pledge.maker && new_status != PledgeStatus.Finished) || 
                  (user.organization == pledge.project.organization && new_status == PledgeStatus.Finished)))
                return Forbid();

            pledge.status = new_status;

            await database.SaveChangesAsync();
            return NoContent();
        }
    }
}
