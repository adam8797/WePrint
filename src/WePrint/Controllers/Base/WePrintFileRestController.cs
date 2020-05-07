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
    public abstract class we_print_file_rest_controller<tt_data, tt_view_model, tt_create_model, tt_key> : we_print_rest_controller<tt_data, tt_view_model, tt_create_model, tt_key>
        where tt_data : class, IIdentifiable<tt_key>
        where tt_view_model : class
        where tt_create_model : class
        where tt_key : struct
    {
        protected we_print_file_rest_controller(IServiceProvider services) : base(services)
        {
        }

        private string get_container_id(tt_data entity)
        {
            return typeof(tt_data).Name.ToLower() + "-" + entity.Id.ToString().SafeFileName();
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
        public async Task<ActionResult<List<file_entry>>> get_files(Guid id)
        {
            var entity = await database.FindAsync<tt_data>(id);
            if (entity == null)
                return NotFound(id);

            if (!await permissions.AllowRead(await current_user, entity))
                return Forbid();

            var files = new List<file_entry>();

            var container = blob_container_provider.GetContainerReference(get_container_id(entity));
            if (!await container.ExistsAsync())
                return files;

            var dir = container.GetDirectoryReference("files");
            var result_segment = await dir.ListBlobsSegmentedAsync(null);
            foreach (var item in result_segment.Results)
            {
                string name;
                switch (item)
                {
                    case CloudBlockBlob blob:
                        name = blob.Name.SafeFileName();
                        files.Add(new file_entry(name, Url.Action("get_file", new { id, filename = name}), 42069));
                        break;

                    case CloudPageBlob blob:
                        name = blob.Name.SafeFileName();
                        files.Add(new file_entry(name, Url.Action("get_file", new { id, filename = name }), 42069));
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
        public async Task<IActionResult> get_file(Guid id, string filename)
        {
            var entity = await database.FindAsync<tt_data>(id);
            if (entity == null)
                return NotFound(id);

            if (!await permissions.AllowRead(await current_user, entity))
                return Forbid();

            var container = blob_container_provider.GetContainerReference(get_container_id(entity));
            if (!await container.ExistsAsync())
                return NotFound();

            var dir = container.GetDirectoryReference("files");
            var blob_ref = dir.GetBlockBlobReference(filename);

            if (!await blob_ref.ExistsAsync())
                return NotFound();

            return File(await blob_ref.OpenReadAsync(), blob_ref.Metadata["MIME"], Path.GetFileName(blob_ref.Name.SafeFileName()));
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
        public async Task<IActionResult> upload_file(Guid id, IFormFile file)
        {
            var entity = await database.FindAsync<tt_data>(id);
            if (entity == null)
                return NotFound(id);

            if (!await permissions.AllowWrite(await current_user, entity))
                return Forbid();

            if (file.Length <= 0)
                return BadRequest("File Length <= 0");

            var max_size_in_megs = configuration.GetValue("FileUploads:MaxSizeInMegabytes", 100.0);
            var max_size_in_bytes = (int)(max_size_in_megs * 1_000_000);

            if (file.Length >= max_size_in_bytes)
                return BadRequest($"File too large. Max size is {max_size_in_bytes} bytes");

            var safe_file_name = file.FileName.SafeFileName();

            var allowed_extensions = configuration.GetSection("FileUploads:AllowedExtensions").Get<string[]>().Select(x => x.ToUpper()).ToList();
            if (!allowed_extensions.Contains(Path.GetExtension(safe_file_name).ToUpper()))
                return BadRequest($"File Extension must be one of: {string.Join(", ", allowed_extensions)}");

            var container = blob_container_provider.GetContainerReference(get_container_id(entity));
            await container.CreateIfNotExistsAsync();

            var dir = container.GetDirectoryReference("files");
            var blob_ref = dir.GetBlockBlobReference(safe_file_name);

            await blob_ref.UploadFromStreamAsync(file.OpenReadStream());

            blob_ref.Metadata["MIME"] = file.ContentType;
            blob_ref.Metadata["Owner"] = (await current_user).Id.ToString();
            await blob_ref.SetMetadataAsync();

            await database.SaveChangesAsync();

            return CreatedAtAction("get_file", new { id, filename = safe_file_name }, null);
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
        public async Task<IActionResult> delete_file(Guid id, string filename)
        {
            var entity = await database.FindAsync<tt_data>(id);
            if (entity == null)
                return NotFound(id);

            if (!await permissions.AllowWrite(await current_user, entity))
                return Forbid();

            var container = blob_container_provider.GetContainerReference(get_container_id(entity));
            if (!await container.ExistsAsync())
                return NotFound();

            var dir = container.GetDirectoryReference("files");
            var blob_ref = dir.GetBlockBlobReference(filename);

            await blob_ref.DeleteIfExistsAsync();

            return NoContent();
        }
    }
}