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

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/organizations")]
    [ProducesResponseType(typeof(object), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(object), StatusCodes.Status403Forbidden)]
    public class OrganizationController : WePrintRestController<Organization, OrganizationViewModel, OrganizationCreateModel, Guid>
    {
        public OrganizationController(IServiceProvider services) : base(services)
        {
        }

        #region REST implementation
        
        protected override async ValueTask<Organization> CreateDataModelAsync(OrganizationCreateModel viewModel)
        {
            var org = Mapper.Map<Organization>(viewModel);
            org.Users.Add(await CurrentUser);
            return org;
        }

        #endregion

        #region Get Full Lists

        [HttpGet("{id}/users")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<UserViewModel>>> GetUsers(Guid id)
        {
            var users = await Database.Users
                .Where(x => x.Organization.Id == id)
                .ProjectTo<UserViewModel>(Mapper.ConfigurationProvider)
                .ToListAsync();
            return users;
        }

        [HttpPost("{id}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> AddUser(Guid id, Guid userId)
        {
            var currentUser = await CurrentUser;

            var organization = await Database.Organizations.FindAsync(id);
            if (organization == null)
                return NotFound(id);

            var targetUser = await Database.Users.FindAsync(userId);
            if (targetUser == null)
                return NotFound(userId);

            if (!organization.Users.Contains(currentUser))
                return Forbid();

            organization.Users.Add(targetUser);

            await Database.SaveChangesAsync();
            return Ok();
        }

        [HttpDelete("{id}/users/{userId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> RemoveUser(Guid id, Guid userId)
        {
            var currentUser = await CurrentUser;
            var organization = await Database.Organizations.FindAsync(id);

            if (organization == null)
                return NotFound(id);

            if (!organization.Users.Contains(currentUser))
                return Forbid();

            var targetUser = organization.Users.SingleOrDefault(user => user.Id == userId);
            if (targetUser == null)
                return NotFound();

            organization.Users.Remove(targetUser);
            await Database.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("{id}/projects")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<List<ProjectViewModel>>> GetProjects(Guid id)
        {
            var projects = await Database.Projects
                .Where(x => x.Organization.Id == id)
                .ProjectTo<ProjectViewModel>(Mapper.ConfigurationProvider)
                .ToListAsync();

            return projects;
        }

        #endregion
    }
}