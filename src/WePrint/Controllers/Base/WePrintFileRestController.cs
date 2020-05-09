using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage.Blob;
using WePrint.Data;
using WePrint.Models;
using WePrint.Utilities;

namespace WePrint.Controllers.Base
{
    public abstract class WePrintFileRestController<TData, TViewModel, TCreateModel, TKey> : WePrintRestController<TData, TViewModel, TCreateModel, TKey>
        where TData : class, IIdentifiable<TKey>
        where TViewModel : class
        where TCreateModel : class
        where TKey : struct
    {
        protected WePrintFileRestController(IServiceProvider services) : base(services)
        {
        }

        private string GetContainerId(TData entity)
        {
            return typeof(TData).Name.ToLower() + "-" + entity.Id.ToString().SafeFileName();
        }

        /// <summary>
        /// Lists files belonging to this entity type
        /// </summary>
        /// <param name="id">ID of the entity</param>
        /// <returns>List of file entries</returns>
        [HttpGet("{id}/files")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<ActionResult<List<FileEntry>>> GetFiles(Guid id)
        {
            var entity = await Database.FindAsync<TData>(id);
            if (entity == null)
                return NotFound(id);

            if (!await Permissions.AllowRead(await CurrentUser, entity))
                return Forbid();

            var files = new List<FileEntry>();

            var container = BlobContainerProvider.GetContainerReference(GetContainerId(entity));
            if (!await container.ExistsAsync())
                return files;

            var dir = container.GetDirectoryReference("files");
            var resultSegment = await dir.ListBlobsSegmentedAsync(null);
            foreach (var item in resultSegment.Results)
            {
                string name;
                switch (item)
                {
                    case CloudBlockBlob blob:
                        name = blob.Name.SafeFileName();
                        files.Add(new FileEntry(name, Url.Action("GetFile", new { id, filename = name}), 42069));
                        break;

                    case CloudPageBlob blob:
                        name = blob.Name.SafeFileName();
                        files.Add(new FileEntry(name, Url.Action("GetFile", new { id, filename = name }), 42069));
                        break;
                }
            }
            return files;
        }

        /// <summary>
        /// Get a particular file from the entity type's storage
        /// </summary>
        /// <param name="id">ID of the owning entity</param>
        /// <param name="filename">Filename to fetch</param>
        /// <returns>Streaming file</returns>
        [HttpGet("{id}/files/{filename}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetFile(Guid id, string filename)
        {
            var entity = await Database.FindAsync<TData>(id);
            if (entity == null)
                return NotFound(id);

            if (!await Permissions.AllowRead(await CurrentUser, entity))
                return Forbid();

            var container = BlobContainerProvider.GetContainerReference(GetContainerId(entity));
            if (!await container.ExistsAsync())
                return NotFound();

            var dir = container.GetDirectoryReference("files");
            var blobRef = dir.GetBlockBlobReference(filename);

            if (!await blobRef.ExistsAsync())
                return NotFound();

            return File(await blobRef.OpenReadAsync(), blobRef.Metadata["MIME"], Path.GetFileName(blobRef.Name.SafeFileName()));
        }

        /// <summary>
        /// Upload a file to this entity type
        /// </summary>
        /// <param name="id">ID of the entity</param>
        /// <param name="file">File from HTML Form</param>
        /// <returns>Status code, and Location if created</returns>
        [HttpPost("{id}/files")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [RequestSizeLimit(100 * 1_000_000)]
        public async Task<IActionResult> UploadFile(Guid id, IFormFile file)
        {
            var entity = await Database.FindAsync<TData>(id);
            if (entity == null)
                return NotFound(id);

            if (!await Permissions.AllowWrite(await CurrentUser, entity))
                return Forbid();

            if (file.Length <= 0)
                return BadRequest("File Length <= 0");

            var maxSizeInMegs = Configuration.GetValue("FileUploads:MaxSizeInMegabytes", 100.0);
            var maxSizeInBytes = (int)(maxSizeInMegs * 1_000_000);

            if (file.Length >= maxSizeInBytes)
                return BadRequest($"File too large. Max size is {maxSizeInBytes} bytes");

            var safeFileName = file.FileName.SafeFileName();

            var allowedExtensions = Configuration.GetSection("FileUploads:AllowedExtensions").Get<string[]>().Select(x => x.ToUpper()).ToList();
            if (!allowedExtensions.Contains(Path.GetExtension(safeFileName).ToUpper()))
                return BadRequest($"File Extension must be one of: {string.Join(", ", allowedExtensions)}");

            var container = BlobContainerProvider.GetContainerReference(GetContainerId(entity));
            await container.CreateIfNotExistsAsync();

            var dir = container.GetDirectoryReference("files");
            var blobRef = dir.GetBlockBlobReference(safeFileName);

            await blobRef.UploadFromStreamAsync(file.OpenReadStream());

            blobRef.Metadata["MIME"] = file.ContentType;
            blobRef.Metadata["Owner"] = (await CurrentUser).Id.ToString();
            await blobRef.SetMetadataAsync();

            await Database.SaveChangesAsync();

            return CreatedAtAction("GetFile", new { id, filename = safeFileName }, null);
        }

        /// <summary>
        /// Delete a file from this entity type
        /// </summary>
        /// <param name="id">ID of the entity type</param>
        /// <param name="filename">Filename to delete</param>
        /// <returns>Status Code</returns>
        [HttpDelete("{id}/files/{filename}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> DeleteFile(Guid id, string filename)
        {
            var entity = await Database.FindAsync<TData>(id);
            if (entity == null)
                return NotFound(id);

            if (!await Permissions.AllowWrite(await CurrentUser, entity))
                return Forbid();

            var container = BlobContainerProvider.GetContainerReference(GetContainerId(entity));
            if (!await container.ExistsAsync())
                return NotFound();

            var dir = container.GetDirectoryReference("files");
            var blobRef = dir.GetBlockBlobReference(filename);

            await blobRef.DeleteIfExistsAsync();

            return NoContent();
        }
    }
}