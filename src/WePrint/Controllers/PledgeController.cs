using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using WePrint.Controllers.Base;
using WePrint.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using WePrint.Models;
using WePrint.Permissions;
using WePrint.Utilities;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/pledges")]
    [Authorize]
    public class PledgeController : WePrintController
    {
        private readonly IPermissionProvider<Pledge, PledgeCreateModel> _pledgePermissionProvider;

        public PledgeController(IServiceProvider services, IPermissionProvider<Pledge, PledgeCreateModel> pledgePermissionProvider) : base(services)
        {
            _pledgePermissionProvider = pledgePermissionProvider;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PledgeViewModel>> GetPledge(Guid id)
        {
            var pledge = await Database.Pledges.FindAsync(id);
            if (pledge == null)
                NotFound(id);

            return Mapper.Map<PledgeViewModel>(pledge);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> UpdateStatus(Guid id, PledgeStatus newStatus)
        {
            var pledge = await Database.Pledges.FindAsync(id);

            var user = await CurrentUser;
            if (!((user == pledge.Maker && newStatus != PledgeStatus.Finished) || (user.Organization == pledge.Project.Organization && newStatus == PledgeStatus.Finished)))
                return Forbid();

            pledge.Status = newStatus;

            await Database.SaveChangesAsync();
            return NoContent();
        }
    }
}
