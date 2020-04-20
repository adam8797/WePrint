using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WePrint.Data;

namespace WePrint.Areas.Admin.Controllers
{
    public abstract class ManagementController<T> : Controller where T: class, IIdentifiable<Guid>
    {
        protected readonly WePrintContext Database;
        protected readonly ILogger Log;

        protected ManagementController(IServiceProvider services)
        {
            Log = (ILogger)services.GetRequiredService(typeof(ILogger<>).MakeGenericType(GetType()));
            Database = services.GetRequiredService<WePrintContext>();
        }

        #region List

        public virtual async Task<IActionResult> Index()
        {
            return View(Database.Set<T>());
        }

        #endregion

        #region Details

        public virtual async Task<IActionResult> Details(Guid id)
        {
            var item = await Database.Set<T>().FindAsync(id);
            if (item == null)
                return NotFound();
            return View(item);
        }

        #endregion

        #region Create

        //ToDo

        public virtual async Task<IActionResult> Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Create(IFormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Edit

        public virtual async Task<IActionResult> Edit(Guid id)
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Edit(Guid id, IFormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        #endregion

        #region Delete

        public virtual async Task<IActionResult> Delete(Guid id)
        {
            var item = await Database.Set<T>().FindAsync(id);
            if (item == null)
                return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Delete(Guid id, IFormCollection collection)
        {
            try
            {
                var item = await Database.Set<T>().FindAsync(id);

                if (item.Deleted)
                {
                    Database.Set<T>().Remove(item);
                    Log.LogInformation($"Attempting to remove {typeof(T).Name} {item.Id} from the database");
                }
                else
                {
                    item.Deleted = true;
                }

                await Database.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error caught while attempting to remove entity");
                throw;
            }
        }

        #endregion

        #region Restore

        public virtual async Task<IActionResult> Restore(Guid id)
        {
            var item = await Database.Set<T>().FindAsync(id);
            if (item == null)
                return NotFound();
            return View(item);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public virtual async Task<IActionResult> Restore(Guid id, IFormCollection collection)
        {
            try
            {
                var item = await Database.Set<T>().FindAsync(id);
                if (item == null)
                    return NotFound();

                item.Deleted = false;
                await Database.SaveChangesAsync();
                return RedirectToAction("Details", new { id });
            }
            catch (Exception ex)
            {
                Log.LogError(ex, "Error caught while attempting to remove entity");
                throw;
            }
        }

        #endregion
    }
}
