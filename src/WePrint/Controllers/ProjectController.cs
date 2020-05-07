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
    [Route("api/projects")]
    [Authorize]
    public class project_controller : we_print_file_rest_controller<project, project_view_model, project_create_model, Guid>
    {
        private readonly IAvatarProvider _avatar;
        private readonly IPermissionProvider<project, project_create_model> _permission;

        public project_controller(IServiceProvider services, IAvatarProvider avatar, IPermissionProvider<project, project_create_model> permission) : base(services)
        {
            _avatar = avatar;
            _permission = permission;
        }

        #region REST Implementation

        protected override async ValueTask<project> create_data_model_async(project_create_model view_model)
        {
            var p = mapper.Map<project>(view_model);
            var user = await current_user;
            if (user.organization == null)
                throw new InvalidOperationException($"Cannot create a project if user {user.UserName} is not a member of an organization");
            p.organization = user.organization;
            p.address = view_model.address != null ? view_model.address : user.organization.address;
            return p;
        }

        #endregion

        [HttpGet("{id}/thumbnail")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> get_thumbnail(Guid id)
        {
            var project = await database.Projects.FindAsync(id);
            return await _avatar.GetAvatarResult(project, false);
        }

        [HttpPost("{id}/thumbnail")]
        [RequestSizeLimit(5 * 1_000_000)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> set_avatar(Guid id, IFormFile posted_image)
        {
            var project = await database.Projects.FindAsync(id);

            if (await _permission.AllowWrite(await current_user, project))
                return await _avatar.SetAvatarResult(project, posted_image);

            return Forbid();
        }
    }
}