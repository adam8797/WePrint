using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using WePrint.Data;
using WePrint.Models;
using WePrint.Permissions;

namespace WePrint.Controllers.Base
{
    /// <remarks>
    /// Route must include {parentId}
    /// </remarks>
    public abstract class WePrintRestSubController<TParent, TCreateParent, TData, TViewModel, TCreateModel, TKey> : WePrintController
        where TData : class, IIdentifiable<TKey>
        where TParent : class, IIdentifiable<TKey>
        where TViewModel : class
        where TCreateModel : class
        where TKey : struct
    {
        protected readonly IPermissionProvider<TParent, TCreateParent> ParentPermissions;
        protected readonly IPermissionProvider<TData, TCreateModel> Permissions;

        protected WePrintRestSubController(IServiceProvider services) : base(services)
        {
            Permissions = services.GetService<IPermissionProvider<TData, TCreateModel>>() ??
                          new DefaultPermissionProvider<TData, TCreateModel>();

            ParentPermissions = services.GetService<IPermissionProvider<TParent, TCreateParent>>() ??
                                new DefaultPermissionProvider<TParent, TCreateParent>();
        }

        #region Virtual Functions


        /// <summary>
        /// Used to filter the Listing request. If done correctly, will translate into a SQL query with LINQ
        /// </summary>
        /// <param name="data">Queryable to filter</param>
        /// <param name="user">Current requesting user</param>
        /// <returns>A queryable to execute</returns>
        protected virtual IQueryable<TData> Filter(IQueryable<TData> data, TParent parent, User user)
        {
            return data;
        }

        /// <summary>
        /// Create a ViewModel from a DB Model. Defaults to just running AutoMapper
        /// </summary>
        /// <param name="data">Database model</param>
        /// <returns>View model</returns>
        protected virtual async ValueTask<TViewModel> CreateViewModelAsync(TData data)
        {
            return Mapper.Map<TViewModel>(data);
        }

        /// <summary>
        /// Create a DB model from a posted Create model. Defaults to running AutoMapper
        /// </summary>
        /// <param name="viewModel">Posted model</param>
        /// <returns>DB Model to store</returns>
        protected virtual async ValueTask<TData> CreateDataModelAsync(TParent parent, TCreateModel viewModel)
        {
            return Mapper.Map<TData>(viewModel);
        }

        /// <summary>
        /// Update a given DB model with a Create model. Defaults to running AutoMapper
        /// </summary>
        /// <param name="dataModel">DB model to update</param>
        /// <param name="viewModel">PUT'ed model</param>
        /// <returns></returns>
        protected virtual async ValueTask<TData> UpdateDataModelAsync(TData dataModel, TCreateModel viewModel)
        {
            return Mapper.Map(viewModel, dataModel);
        }

        /// <summary>
        /// Update a given DB model with a view model. Defaults to running AutoMapper
        /// </summary>
        /// <param name="dataModel">DB Model to update</param>
        /// <param name="viewModel">Updated view model</param>
        /// <returns></returns>
        protected virtual async ValueTask<TData> UpdateDataModelAsync(TData dataModel, TViewModel viewModel)
        {
            return Mapper.Map(viewModel, dataModel);
        }

        #endregion

        #region HTTP Verbs

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        // GET /api/[controller]
        public virtual async Task<ActionResult<IEnumerable<TViewModel>>> Get(TKey parentId)
        {
            var valid = new List<TViewModel>();
            var user = await CurrentUser;

            var parent = await Database.FindAsync<TParent>(parentId);
            if (parent == null)
                return NotFound();

            foreach (var entity in Filter(Database.Set<TData>(), parent, user).Where(e => !e.Deleted))
            {
                if (await Permissions.AllowRead(user, entity))
                    valid.Add(await CreateViewModelAsync(entity));
            }
            return valid;
        }


        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // GET /api/[controller]/{id}
        public virtual async Task<ActionResult<TViewModel>> Get(TKey parentId, TKey id)
        {
            var entity = await Database.Set<TData>().FindAsync(id);

            if (entity == null)
                return NotFound(id);

            if (await Permissions.AllowRead(await CurrentUser, entity))
                return await CreateViewModelAsync(entity);

            return Forbid();
        }


        [HttpPost]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        // POST /api/[controller]
        public virtual async Task<ActionResult<TViewModel>> Post(TKey parentId, [FromBody] TCreateModel body)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var parent = await Database.FindAsync<TParent>(parentId);
            if (parent == null)
                return NotFound();

            if (!await Permissions.AllowCreate(await CurrentUser, body))
                return Forbid();

            var dataModel = await CreateDataModelAsync(parent, body);
            dataModel.Deleted = false;

            Database.Set<TData>().Add(dataModel);
            await Database.SaveChangesAsync();

            return CreatedAtAction("Get", new { parentId, id = dataModel.Id }, await CreateViewModelAsync(dataModel));
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        // PUT /api/[controller]/{id}
        public virtual async Task<ActionResult<TViewModel>> Put(TKey parentId, TKey id, [FromBody] TCreateModel create)
        {
            var set = Database.Set<TData>();
            var entity = await set.FindAsync(id);

            if (entity == null)
                return NotFound();

            if (!await Permissions.AllowWrite(await CurrentUser, entity) || entity.Deleted)
                return Forbid();

            await UpdateDataModelAsync(entity, create);
            await Database.SaveChangesAsync();
            return await CreateViewModelAsync(entity);
        }


        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // PATCH /api/[controller]/{id}
        public virtual async Task<ActionResult<TViewModel>> Patch(TKey parentId, TKey id, [FromBody] JsonPatchDocument<TViewModel> patch)
        {
            var entity = await Database.Set<TData>().FindAsync(id);

            if (entity == null)
                return NotFound(id);

            if (!await Permissions.AllowWrite(await CurrentUser, entity) || entity.Deleted)
                return Forbid();

            var dto = await CreateViewModelAsync(entity);
            patch.ApplyTo(dto);
            await UpdateDataModelAsync(entity, dto);

            await Database.SaveChangesAsync();
            return Ok(await CreateViewModelAsync(entity));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        // DELETE /api/[controller]/{id}
        public virtual async Task<IActionResult> Delete(TKey parentId, TKey id)
        {
            var entity = await Database.Set<TData>().FindAsync(id);
            if (entity == null)
                return NotFound();

            var parent = await Database.FindAsync<TParent>(parentId);
            if (parent == null)
                return NotFound();

            if (!await Permissions.AllowWrite(await CurrentUser, entity))
                return Forbid();

            entity.Deleted = true;
            await Database.SaveChangesAsync();
            return Ok();
        }

        #endregion HTTP Verbs
    }
}
