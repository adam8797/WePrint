using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        protected override DbSet<Organization> GetDbSet(WePrintContext database) => database.Organizations;

        protected override async ValueTask<bool> AllowCreate(User user, OrganizationCreateModel create)
        {
            return user.Organization == null;
        }

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