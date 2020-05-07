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
    public class pledge_controller : we_print_controller
    {
        private readonly IPermissionProvider<pledge, pledge_create_model> _pledge_permission_provider;

        public pledge_controller(IServiceProvider services, IPermissionProvider<pledge, pledge_create_model> pledge_permission_provider) : base(services)
        {
            _pledge_permission_provider = pledge_permission_provider;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<pledge_view_model>> get_pledge(Guid id)
        {
            var pledge = await database.Pledges.FindAsync(id);
            if (pledge == null)
                NotFound(id);

            return mapper.Map<pledge_view_model>(pledge);
        }

        [HttpPatch("{id}")]
        public async Task<IActionResult> update_status(Guid id, PledgeStatus new_status)
        {
            var pledge = await database.Pledges.FindAsync(id);

            var user = await current_user;
            if (!((user == pledge.maker && new_status != PledgeStatus.Finished) || (user.organization == pledge.project.organization && new_status == PledgeStatus.Finished)))
                return Forbid();

            pledge.status = new_status;

            await database.SaveChangesAsync();
            return NoContent();
        }
    }
}
