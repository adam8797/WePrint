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
using WePrint.Data;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/project")]
    [Authorize]
    public class OrganizationController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public OrganizationController(IServiceProvider services, IConfiguration configuration) : base(services)
        {
            _configuration = configuration;
        }

        #region Base organization methods
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Organization>>> GetOrganizations()
        {
            return await Database.Organizations.ToArrayAsync();
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Organization>> GetOrganization(Guid id)
        {
            return await Database.Organizations.FindAsync(id);
        }

        [HttpPost]
        public async Task<ActionResult<Organization>> CreateOrganization([FromBody] Organization enteredOrg)
        {
            var currentUser = await CurrentUser;
            
            if (!enteredOrg.Users.Contains(currentUser))
                enteredOrg.Users.Add(currentUser);

            Database.Organizations.Add(enteredOrg);
            await Database.SaveChangesAsync();

            return CreatedAtAction("GetOrganization", new {id = enteredOrg.Id}, enteredOrg);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<Organization>> AddOrUpdateOrganization(Guid id, [FromBody] Organization update)
        {
            var currentUser = await CurrentUser;
            var org = await Database.Organizations.FindAsync(id);

            if (org == null)
            {
                if (!update.Users.Contains(currentUser))
                    update.Users.Add(currentUser);

                Database.Organizations.Add(update);
            }
            else
            {
                if (!org.Users.Contains(currentUser))    
                    return Forbid();

                Database.Organizations.Update(update);
            }

            await Database.SaveChangesAsync();
            return update;
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Organization>> PatchOrganization(Guid id, [FromBody] JsonPatchDocument<Organization> patchDocument)
        {
            var currentUser = await CurrentUser;
            var org = await Database.Organizations.FindAsync(id);

            if (org == null)
                return NotFound();
            if (!org.Users.Contains(currentUser))    
                return Forbid();

            patchDocument.ApplyTo(org);

            await Database.SaveChangesAsync();
            return org;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteOrganization(Guid id)
        {
            var currentUser = await CurrentUser;
            var org = await Database.Organizations.FindAsync(id);

            if (org == null)
                return NotFound();
            if (!org.Users.Contains(currentUser))    
                return Forbid();

            Database.Organizations.Remove(org);
            await Database.SaveChangesAsync();
            return Ok();
        }

        
        #endregion
    }
}