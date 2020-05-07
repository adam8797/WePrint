using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WePrint.Controllers.Base;
using WePrint.Data;
using WePrint.Models;
using WePrint.Utilities;

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/organizations")]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public class organization_controller : we_print_rest_controller<organization, organization_view_model, organization_create_model, Guid>
    {
        private readonly IAvatarProvider _avatar;

        public organization_controller(IServiceProvider services, IAvatarProvider avatar) : base(services)
        {
            _avatar = avatar;
        }

        #region REST implementation
        
        protected override async ValueTask<organization> create_data_model_async(organization_create_model view_model)
        {
            var org = mapper.Map<organization>(view_model);
            org.users.Add(await current_user);
            return org;
        }

        protected override async Task post_delete_data_model_async(organization data_model)
        {
            data_model.users.Clear();
        }

        #endregion

        #region Avatars

        [AllowAnonymous]
        [HttpGet("{id}/avatar")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> get_org_avatar(Guid id)
        {
            var org = await database.Organizations.FindAsync(id);
            if (org == null)
                return NotFound();

            return await _avatar.GetAvatarResult(org);
        }

        [HttpPost("{id}/avatar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> set_org_avatar(Guid id, IFormFile upload)
        {
            var org = await database.Organizations.FindAsync(id);
            if (org == null)
                return NotFound();

            if (!await permissions.AllowWrite(await current_user, org))
                return Forbid();

            return await _avatar.SetAvatarResult(org, upload);
        }

        [HttpDelete("{id}/avatar")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> clear_org_avatar(Guid id)
        {
            var org = await database.Organizations.FindAsync(id);
            if (org == null)
                return NotFound();

            if (!await permissions.AllowWrite(await current_user, org))
                return Forbid();

            return await _avatar.ClearAvatar(org);
        }


        #endregion

        #region Get Full Lists

        [HttpGet("{id}/users")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<user_view_model>>> get_users(Guid id)
        {
            var users = mapper.Map<List<user_view_model>>(
                await database.Users
                .Where(x => x.organization.Id == id)
                .ToListAsync());
            return users;
        }

        [HttpPost("{id}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> add_user(Guid id, Guid user_id)
        {
            var current_user = await ((we_print_controller) this).current_user;

            var organization = await database.Organizations.FindAsync(id);
            if (organization == null)
                return NotFound(id);

            var target_user = await database.Users.FindAsync(user_id);
            if (target_user == null)
                return NotFound(user_id);

            if (!organization.users.Contains(current_user))
                return Forbid();

            organization.users.Add(target_user);

            await database.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> remove_user(Guid id, Guid user_id)
        {
            var current_user = await ((we_print_controller) this).current_user;
            var organization = await database.Organizations.FindAsync(id);

            if (organization == null)
                return NotFound(id);

            if (!organization.users.Contains(current_user))
                return Forbid();

            var target_user = organization.users.SingleOrDefault(user => user.Id == user_id);
            if (target_user == null)
                return NotFound();

            organization.users.Remove(target_user);
            await database.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}/projects")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<project_view_model>>> get_projects(Guid id)
        {
            // This is ugly, but for some reason this needs to be evaluated client side.
            var projects = await database.Projects
                .Where(x => x.organization.Id == id)
                .ToListAsync();

            var view_models = mapper.Map<List<project_view_model>>(projects);
            return view_models;
        }

        #endregion
    }
}