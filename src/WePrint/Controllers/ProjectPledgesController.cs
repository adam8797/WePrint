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
    public class ProjectPledgesController : WePrintRestSubController<Project, ProjectCreateModel, Pledge, PledgeViewModel, PledgeCreateModel, Guid>
    {
        public ProjectPledgesController(IServiceProvider services) : base(services)
        {
        }

        protected override async ValueTask<Pledge> CreateDataModelAsync(Project parent, PledgeCreateModel viewModel)
        {
            var pledge = Mapper.Map<Pledge>(viewModel);
            pledge.Maker = await CurrentUser;
            pledge.Project = parent;
            pledge.Status = PledgeStatus.NotStarted;
            pledge.Created = DateTimeOffset.Now;
            return pledge;
        }

        protected override IQueryable<Pledge> Filter(IQueryable<Pledge> data, Project parent, User user)
        {
            return Database.Pledges.Where(x => x.Project == parent);
        }

        [HttpPatch("{id}/setstatus")]
        public async Task<IActionResult> UpdateStatus(Guid parentId, Guid id, PledgeStatus newStatus)
        {
            var pledge = await Database.Pledges.FindAsync(id);

            if (pledge.Project.Id != parentId)
                return NotFound();

            var user = await CurrentUser;
            if (!((user == pledge.Maker && newStatus != PledgeStatus.Finished) || 
                  (user.Organization == pledge.Project.Organization && newStatus == PledgeStatus.Finished)))
                return Forbid();

            pledge.Status = newStatus;

            await Database.SaveChangesAsync();
            return NoContent();
        }
    }
}
