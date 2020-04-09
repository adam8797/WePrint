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
    public class ProjectController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public ProjectController(IServiceProvider services, IConfiguration configuration) : base(services)
        {
            _configuration = configuration;
        }

        #region Base Project Methods

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Project>>> GetProjects()
        {
            var projects = await Database.Projects.ToArrayAsync();
            return projects;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Project>> GetProject(Guid id)
        {
            var project = await Database.Projects.FindAsync(id);

            return project;
        }

        [HttpPost]
        public async Task<ActionResult<Project>> CreateProject([FromBody] Project enteredProject)
        {
            var currentUser = await CurrentUser;
            if (currentUser.Organization == null)
                return Forbid();

            enteredProject.Organization = currentUser.Organization;
            Database.Projects.Add(enteredProject);
            await Database.SaveChangesAsync();

            return CreatedAtAction("GetProject", new {id = enteredProject.Id}, enteredProject);
        }
        
        [HttpPut("{id}")]
        public async Task<ActionResult<Project>> UpdateProject(Guid id, [FromBody] Project update)
        {
            var currentUser = await CurrentUser;
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
            {
                if (currentUser.Organization == null)
                    return Forbid();

                update.Id = id;
                Database.Projects.Add(update);
            }
            else
            {
                if (currentUser.Organization.Id != project.Organization.Id)
                    return Forbid();
                
                Database.Projects.Update(update);
            }

            await Database.SaveChangesAsync();
            return project;
        }

        [HttpPatch("{id}")]
        public async Task<ActionResult<Project>> PatchProject(Guid id, [FromBody] JsonPatchDocument<Project> patchDocument)
        {
            var user = await CurrentUser;
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);
            
            if (user.Organization.Id != project.Organization.Id)
                return Forbid();

            patchDocument.ApplyTo(project);

            await Database.SaveChangesAsync();

            return project;
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteProject(Guid id)
        {
            var currentUser = await CurrentUser;
            var project  = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);

            if (currentUser.Organization.Id != project.Organization.Id)
                return Forbid();

            Database.Projects.Remove(project);
            await Database.SaveChangesAsync();
            return Ok();
        }
        #endregion

        /*#region Files
        [HttpGet("{id}/files")]
        [SwaggerOperation(Tags = new[]{"Files"})]
        public async Task<IActionResult> GetFiles(Guid id)
        {
            var project = await Database.Jobs.FindAsync(id);
            if (project == null)
                return NotFound(id);
            
            //return Ok(project.Attachments.Select());
        }

        #endregion*/

        #region Updates
        [HttpGet("{id}/updates")]
        [SwaggerOperation(Tags = new []{"Updates"})]
        public async Task<ActionResult<IList<ProjectUpdate>>> ListUpdates(Guid id)
        {
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);
            
            return Ok(project.Updates);
        }

        [HttpGet("{id}/updates/{updateId}")]
        [SwaggerOperation(Tags = new []{"Updates"})]
        public async Task<ActionResult<ProjectUpdate>> GetUpdate(Guid id, Guid updateId)
        {
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);

            var update = project.Updates.SingleOrDefault(x => x.Id == updateId);

            if (update == null) 
                return NotFound();

            return update;
        }

        [HttpPost("{id}/updates")]
        [SwaggerOperation(Tags = new []{"Updates"})]
        public async Task<IActionResult> AddUpdate(Guid id, ProjectUpdate update)
        {
            var user = await CurrentUser;
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);

            if (user.Organization.Id != project.Organization.Id)
                return Forbid();

            update.PostedBy = user;
            update.Timestamp = DateTime.Now;

            project.Updates.Add(update);

            await Database.SaveChangesAsync();

            return CreatedAtAction("GetUpdate", new {id, updateId = update.Id}, update);
        }

        // I'm going to call things updateUpdate
        // if anyone has a problem with that I'm happy to duel over it
        [HttpPut("{id}/updates/{updateId}")]
        [SwaggerOperation(Tags = new []{"Updates"})]
        public async Task<IActionResult> AddOrUpdateUpdate(Guid id, Guid updateId, ProjectUpdate updateUpdate)
        {
            var user = await CurrentUser;
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);

            if (user.Organization.Id != project.Organization.Id)
                return Forbid();

            updateUpdate.PostedBy = user;
            updateUpdate.Timestamp = DateTime.Now;

            var existingUpdate = project.Updates.SingleOrDefault(x => x.Id == updateId);
            if (existingUpdate != null)
            {
                existingUpdate.Body = updateUpdate.Body;
                existingUpdate.Title = updateUpdate.Title;
                existingUpdate.Timestamp = updateUpdate.Timestamp;
            }
            else{
                project.Updates.Add(updateUpdate);
            }

            await Database.SaveChangesAsync();

            return NoContent();
        }

        [HttpDelete("{id}/updates/{updateId}")]
        [SwaggerOperation(Tags = new []{"Updates"})]
        public async Task<IActionResult> DeleteComment(Guid id, Guid updateId)
        {
            var user = await CurrentUser;
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);
            
            if (user.Organization.Id != project.Organization.Id)
                return Forbid();

            var update = project.Updates.SingleOrDefault(x => x.Id == updateId);

            if (update == null)
                return NotFound();
            
            project.Updates.Remove(update);

            await Database.SaveChangesAsync();
            
            return Ok();
        }

        #endregion

        #region Pledges

        [HttpGet("{id}/pledges")]
        [SwaggerOperation(Tags = new[]{"Pledges"})]
        public async Task<ActionResult<IList<Pledge>>> ListPledges(Guid id)
        {
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);

            return Ok(project.Pledges);
        }

        [HttpGet("{id}/pledges/{pledgeId}")]
        [SwaggerOperation(Tags = new[]{"Pledges"})]
        public async Task<ActionResult<Bid>> GetPledge(Guid id, Guid pledgeId)
        {
            var project = await Database.Projects.FindAsync(id);

            if (project == null)
                return NotFound(id);

            var pledge = project.Pledges.SingleOrDefault(x => x.Id == pledgeId);

            if (pledge == null)
                return NotFound();
            
            return Ok(pledge);
        }

        [HttpPost("{id}/pledges")]
        [SwaggerOperation(Tags = new[]{"Pledges"})]
        public async Task<IActionResult> AddPledge(Guid id, [FromBody] Pledge pledge)
        {
            var project = await Database.Projects.FindAsync(id);
            var user = await CurrentUser;

            if (project == null)
                return NotFound(id);

            if (user == null || user.Printers == null || !user.Printers.Any())
                return Forbid();

            pledge.Maker = user;
            project.Pledges.Add(pledge);

            await Database.SaveChangesAsync();

            return CreatedAtAction("GetPledge", new {id, pledgeId = pledge.Id}, pledge);
        }

        [HttpPut("{id}/pledges/{pledgeId}")]
        [SwaggerOperation(Tags = new[]{"Pledges"})]
        public async Task<IActionResult> AddOrUpdatePledge(Guid id, Guid pledgeId, [FromBody] Pledge update)
        {
            var project = await Database.Projects.FindAsync(id);
            var user = await CurrentUser;

            if (project == null)
                return NotFound(id);

            var pledge = project.Pledges.SingleOrDefault(x => x.Id == pledgeId);

            if (user == null || user.Printers == null || !user.Printers.Any())
                return Forbid();
            update.Maker = user;

            if (pledge == null)
            {
                project.Pledges.Add(update);
            }
            else
            {
                update.Id = pledge.Id;
                Database.Update(update);
            }

            await Database.SaveChangesAsync();
            return NoContent();
        }

        [HttpPatch("{id}/pledges/{pledgeId}")]
        [SwaggerOperation(Tags = new[]{"Pledges"})]
        public async Task<ActionResult<Pledge>> PatchPledge(Guid id, Guid pledgeId, [FromBody] JsonPatchDocument patchDocument)
        {
            var project = await Database.Projects.FindAsync(id);
            var user = await CurrentUser;

            if (project == null)
                return NotFound(id);

            var pledge = project.Pledges.SingleOrDefault(x => x.Id == pledgeId);

            if (pledge == null)
                return NotFound();

            if (user != pledge.Maker)
                return Forbid();
                
            patchDocument.ApplyTo(pledge);

            await Database.SaveChangesAsync();
            
            return pledge;
        }

        [HttpDelete("{id}/pledges/{pledgeId}")]
        [SwaggerOperation(Tags = new[]{"Pledges"})]
        public async Task<IActionResult> DeletePledge(Guid id, Guid pledgeId)
        {
            var project = await Database.Projects.FindAsync(id);
            var user = await CurrentUser;

            if (project == null)
                return NotFound(id);

            var pledge = project.Pledges.SingleOrDefault(x => x.Id == pledgeId);

            if (pledge == null)
                return NotFound();

            if (user != pledge.Maker)
                return Forbid();

            project.Pledges.Remove(pledge);

            await Database.SaveChangesAsync();

            return Ok();
        }

        #endregion
    }
}