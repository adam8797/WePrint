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
    public class ProjectController : WePrintFileRestController<Project, ProjectViewModel, ProjectCreateModel, Guid>
    {
        private readonly IAvatarProvider _avatar;
        private readonly IPermissionProvider<Project, ProjectCreateModel> _permission;

        public ProjectController(IServiceProvider services, IAvatarProvider avatar, IPermissionProvider<Project, ProjectCreateModel> permission) : base(services)
        {
            _avatar = avatar;
            _permission = permission;
        }

        #region REST Implementation

        protected override async ValueTask<Project> CreateDataModelAsync(ProjectCreateModel viewModel)
        {
            var p = Mapper.Map<Project>(viewModel);
            var user = await CurrentUser;
            if (user.Organization == null)
                throw new InvalidOperationException($"Cannot create a project if user {user.UserName} is not a member of an organization");
            p.Organization = user.Organization;
            p.Address = viewModel.Address != null ? viewModel.Address : user.Organization.Address;
            return p;
        }

        #endregion

        [HttpGet("{id}/thumbnail")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetThumbnail(Guid id)
        {
            var project = await Database.Projects.FindAsync(id);
            return await _avatar.GetAvatarResult(project, false);
        }

        [HttpPost("{id}/thumbnail")]
        [RequestSizeLimit(5 * 1_000_000)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> SetAvatar(Guid id, IFormFile postedImage)
        {
            var project = await Database.Projects.FindAsync(id);

            if (await _permission.AllowWrite(await CurrentUser, project))
                return await _avatar.SetAvatarResult(project, postedImage);

            return Forbid();
        }
    }
}