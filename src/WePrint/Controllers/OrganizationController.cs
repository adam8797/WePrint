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
using WePrint.Models.Organization;
using WePrint.Models.User;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/organizations")]
    [Authorize]
    public class OrganizationController : RESTController<Organization, OrganizationViewModel, OrganizationCreateModel, Guid>
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
        public async Task<ActionResult<IEnumerable<User>>> GetUsers(Guid id)
        {
            var org = await Database.Organizations.FindAsync(id);

            if (org == null)
                return NotFound(id);

            return Ok(org.Users);
        }

        [HttpGet("{id}/projects")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<User>>> GetProjects(Guid id)
        {
            var org = await Database.Organizations.FindAsync(id);

            if (org == null)
                return NotFound(id);

            return Ok(org.Projects);
        }

        #endregion
    }
}