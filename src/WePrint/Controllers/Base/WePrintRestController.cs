using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Bson;
using WePrint.Data;
using WePrint.Models;
using WePrint.Permissions;

namespace WePrint.Controllers.Base
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class we_print_rest_controller<tt_data, tt_view_model, tt_create_model, tt_key> : we_print_controller 
        where tt_data: class, IIdentifiable<tt_key>
        where tt_view_model: class 
        where tt_create_model: class
        where tt_key: struct
    {
        protected readonly IPermissionProvider<tt_data, tt_create_model> permissions;

        protected we_print_rest_controller(IServiceProvider services) : base(services)
        {
            permissions = services.GetService<IPermissionProvider<tt_data, tt_create_model>>() ??
                                  new DefaultPermissionProvider<tt_data, tt_create_model>();
        }

        #region Virtual Functions

        /// <summary>
        /// Used to filter the Listing request. If done correctly, will translate into a SQL query with LINQ
        /// </summary>
        /// <param name="data">Queryable to filter</param>
        /// <param name="user">Current requesting user</param>
        /// <returns>A queryable to execute</returns>
        protected virtual IQueryable<tt_data> filter(IQueryable<tt_data> data, user user)
        {
            return data;
        }

        /// <summary>
        /// Create a ViewModel from a DB Model. Defaults to just running AutoMapper
        /// </summary>
        /// <param name="data">Database model</param>
        /// <returns>View model</returns>
        protected virtual async ValueTask<tt_view_model> create_view_model_async(tt_data data)
        {
            return mapper.Map<tt_view_model>(data);
        }

        /// <summary>
        /// Create a DB model from a posted Create model. Defaults to running AutoMapper
        /// </summary>
        /// <param name="view_model">Posted model</param>
        /// <returns>DB Model to store</returns>
        protected virtual async ValueTask<tt_data> create_data_model_async(tt_create_model view_model)
        {
            return mapper.Map<tt_data>(view_model);
        }

        /// <summary>
        /// Update a given DB model with a Create model. Defaults to running AutoMapper
        /// </summary>
        /// <param name="data_model">DB model to update</param>
        /// <param name="view_model">PUT'ed model</param>
        /// <returns></returns>
        protected virtual async ValueTask<tt_data> update_data_model_async(tt_data data_model, tt_create_model view_model)
        {
            return mapper.Map(view_model, data_model);
        }

        /// <summary>
        /// Update a given DB model with a view model. Defaults to running AutoMapper
        /// </summary>
        /// <param name="data_model">DB Model to update</param>
        /// <param name="view_model">Updated view model</param>
        /// <returns></returns>
        protected virtual async ValueTask<tt_data> update_data_model_async(tt_data data_model, tt_view_model view_model)
        {
            return mapper.Map(view_model, data_model);
        }

        /// <summary>
        /// Take post delete actions if needed.
        /// </summary>
        /// <param name="data_model">DB Model to delete</param>
        /// <returns></returns>
        protected virtual async Task post_delete_data_model_async(tt_data data_model)
        {
        }

        #endregion

        #region HTTP Verbs

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // GET /api/[controller]
        public virtual async Task<ActionResult<IEnumerable<tt_view_model>>> get()
        {
            var valid = new List<tt_view_model>();
            var user = await current_user;
            foreach (var entity in filter(database.Set<tt_data>(), user).Where(e => !e.Deleted))
            {
                if (await permissions.AllowRead(user, entity))
                    valid.Add(await create_view_model_async(entity));
            }
            return valid;
        }


        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // GET /api/[controller]/{id}
        public virtual async Task<ActionResult<tt_view_model>> get(tt_key id)
        {
            var entity = await database.Set<tt_data>().FindAsync(id);

            if (entity == null)
                return NotFound(id);

            if (await permissions.AllowRead(await current_user, entity))
                return await create_view_model_async(entity);

            return Forbid();
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        // POST /api/[controller]
        public virtual async Task<ActionResult<tt_view_model>> post([FromBody] tt_create_model body)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            if (!await permissions.AllowCreate(await current_user, body))
                return Forbid();

            var data_model = await create_data_model_async(body);
            data_model.Deleted = false;

            database.Set<tt_data>().Add(data_model);
            await database.SaveChangesAsync();

            return CreatedAtAction("get", new { id = data_model.Id }, await create_view_model_async(data_model));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        // PUT /api/[controller]/{id}
        public virtual async Task<ActionResult<tt_view_model>> put(tt_key id, [FromBody] tt_create_model create)
        {
            var set = database.Set<tt_data>();
            var entity = await set.FindAsync(id);

            if (entity == null)
                return NotFound();

            if (!await permissions.AllowWrite(await current_user, entity) || entity.Deleted)
                return Forbid();

            await update_data_model_async(entity, create);
            await database.SaveChangesAsync();
            return await create_view_model_async(entity);
        }


        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // PATCH /api/[controller]/{id}
        public virtual async Task<ActionResult<tt_view_model>> patch(tt_key id, [FromBody] JsonPatchDocument<tt_view_model> patch)
        {
            var entity = await database.Set<tt_data>().FindAsync(id);

            if (entity == null)
                return NotFound(id);

            if (!await permissions.AllowWrite(await current_user, entity) || entity.Deleted)
                return Forbid();

            var dto = await create_view_model_async(entity);
            patch.ApplyTo(dto);
            await update_data_model_async(entity, dto);

            await database.SaveChangesAsync();
            return Ok(await create_view_model_async(entity));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // DELETE /api/[controller]/{id}
        public virtual async Task<IActionResult> delete(tt_key id)
        {
            var entity = await database.Set<tt_data>().FindAsync(id);

            if (entity == null)
                return NotFound();

            if (!await permissions.AllowWrite(await current_user, entity))
                return Forbid();

            entity.Deleted = true;
            await post_delete_data_model_async(entity);
            await database.SaveChangesAsync();
            return Ok();
        }

        #endregion HTTP Verbs
    }
}