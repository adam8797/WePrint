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
using WePrint.Models.Project;
using WePrint.Models.User;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectController : RESTController<Project, ProjectViewModel, ProjectCreateModel, Guid>
    {
        public ProjectController(IServiceProvider services, IConfiguration configuration) : base(services)
        {
        }

        protected override DbSet<Project> GetDbSet(WePrintContext database) => database.Projects;

        #region REST Implementation

        protected override async ValueTask<bool> AllowWrite(User user, Project entity)
        {
            return user.Organization == entity.Organization;
        }

        protected override async ValueTask<Project> CreateDataModelAsync(ProjectCreateModel viewModel)
        {
            var p = Mapper.Map<Project>(viewModel);
            var user = await CurrentUser;
            if (user.Organization == null)
                throw new InvalidOperationException($"Cannot create a project if user {user.UserName} is not a member of an organization");
            p.Organization = user.Organization;
            return p;
        }

        protected override async ValueTask<bool> AllowCreate(User user, ProjectCreateModel create)
        {
            return user.Organization != null;
        }

        #endregion

        #region Files

        [HttpGet("{id}/files")]
        public async Task<ActionResult<List<(string Name, long Length)>>> GetFiles(Guid id)
        {
            var project = await Database.Projects.FindAsync(id);
            if (project == null)
                return NotFound(id);

            if (!await AllowRead(await CurrentUser, project))
                return Forbid();

            var files = new List<(string Name, long Length)>();

            var container = GetBlobContainer(project.Id.ToString("N"));
            if (!await container.ExistsAsync())
                return files;

            var dir = container.GetDirectoryReference("files");
            var resultSegment = await dir.ListBlobsSegmentedAsync(null);

            foreach (var item in resultSegment.Results)
            {
                switch (item)
                {
                    case CloudBlockBlob blob:
                        files.Add((blob.Name, 42069));
                        break;

                    case CloudPageBlob blob:
                        files.Add((blob.Name, 42069));
                        break;
                }
            }
            return files;
        }

        [HttpGet("{id}/files/{filename}")]
        public async Task<IActionResult> GetFile(Guid id, string filename)
        {
            var project = await Database.Projects.FindAsync(id);
            if (project == null)
                return NotFound(id);

            if (!await AllowRead(await CurrentUser, project))
                return Forbid();

            var container = GetBlobContainer(project.Id.ToString("N"));
            if (!await container.ExistsAsync())
                return NotFound();

            var dir = container.GetDirectoryReference("files");
            var blobRef = dir.GetBlockBlobReference(filename);

            if (!await blobRef.ExistsAsync())
                return NotFound();

            return File(await blobRef.OpenReadAsync(), blobRef.Metadata["MIME"], blobRef.Name);
        }

        [HttpPost("{id}/files")]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var project = await Database.Projects.FindAsync(id);
            if (project == null)
                return NotFound(id);

            if (!await AllowWrite(await CurrentUser, project))
                return Forbid();

            if (file.Length <= 0)
                return BadRequest("File Length <= 0");

            var maxSizeInMegs = Configuration.GetValue("FileUploads:MaxSizeInMegabytes", 100.0);
            var maxSizeInBytes = (int)(maxSizeInMegs * 1_000_000);

            if (file.Length >= maxSizeInBytes)
                return BadRequest($"File too large. Max size is {maxSizeInBytes} bytes");

            var allowedExtensions = Configuration.GetSection("FileUploads:AllowedExtensions").Get<string[]>();
            if (allowedExtensions != null && !allowedExtensions.Contains(Path.GetExtension(file.FileName)))
                return BadRequest($"File Extension must be one of: {string.Join(", ", allowedExtensions)}");

            var container = GetBlobContainer(project.Id.ToString("N"));
            await container.CreateIfNotExistsAsync();

            var dir = container.GetDirectoryReference("files");
            var blobRef = dir.GetBlockBlobReference(file.FileName);

            await blobRef.UploadFromStreamAsync(file.OpenReadStream());

            blobRef.Metadata["MIME"] = file.ContentType;
            await blobRef.SetMetadataAsync();

            project.Attachments.Add(new ProjectAttachment()
            {
                SubmittedBy = await CurrentUser,
                // using file.FileName could/probably-is dangerous and we should not use it
                URL = Url.Action("GetFile", new { id, filename = file.FileName })
            });

            await Database.SaveChangesAsync();

            return CreatedAtAction("GetFile", new { id, filename = file.FileName }, new { Name = file.FileName, Size = file.Length });
        }

        [HttpDelete("{id}/files/{filename}")]
        public async Task<IActionResult> DeleteFile(Guid id, string filename)
        {
            var project = await Database.Projects.FindAsync(id);
            if (project == null)
                return NotFound(id);

            if (!await AllowRead(await CurrentUser, project))
                return Forbid();

            var container = GetBlobContainer(project.Id.ToString("N"));
            if (!await container.ExistsAsync())
                return NotFound();

            var dir = container.GetDirectoryReference("files");
            var blobRef = dir.GetBlockBlobReference(filename);

            await blobRef.DeleteIfExistsAsync();

            return NoContent();
        }

        #endregion

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