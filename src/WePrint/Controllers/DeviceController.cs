using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using WePrint.Common.ServiceDiscovery;
using WePrint.Common.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/device")]
    public class DeviceController : WePrintController
    {
        public DeviceController(ILogger log, IAsyncDocumentSession database) : base(log, database)
        {
        }

        // GET: /api/Device/
        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            var user = await CurrentUser;
            if (user == null) return Unauthorized();

            await InitUserPrinters(user);
            
            return Json(user.Printers);
        }

        // GET" /api/Device/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceByID([FromRoute]string id)
        {
            var user = await CurrentUser;
            if (user == null) return Unauthorized();

            await InitUserPrinters(user);

            var printer = user.Printers.FirstOrDefault(x => x.Id == id);
            if (printer == null) return NotFound();

            return Json(printer);
        }

        // POST: /api/Device/
        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody]PrinterModel model)
        {
            var user = await CurrentUser;

            if (user == null)
                return Unauthorized();

            await InitUserPrinters(user);

            user.Printers.Add(model);

            await Database.StoreAsync(model);
            await Database.StoreAsync(user);
            await Database.SaveChangesAsync();

            return Ok(model.Id);
        }

        // PUT: /api/Device/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice([FromRoute]string id, [FromBody] PrinterUpdateModel model)
        {
            var user = await CurrentUser;

            if (user == null)
                return Unauthorized();

            var Device = user.Printers.FirstOrDefault(p => p.Id == id);

            if (Device == null)
                return NotFound("Device " + id + " not found");

            Common.ReflectionHelper.CopyPropertiesTo(model, Device);

            await Database.StoreAsync(user);
            await Database.StoreAsync(Device);
            await Database.SaveChangesAsync();

            return Ok();
        }

        // DELETE: /api/Device/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice([FromRoute] string id)
        {
            var user = await CurrentUser;

            if (user == null)
                return Unauthorized();

            var Device = user.Printers.FirstOrDefault(p => p.Id == id);

            if (Device == null)
                return NotFound("Device " + id + " not found");

            user.Printers.Remove(Device);

            await Database.StoreAsync(user);
            await Database.SaveChangesAsync();

            return Ok();
        }

        private async Task InitUserPrinters(ApplicationUser user)
        {
            if (user.Printers != null)
                return;
            else
            {
                user.Printers = new List<PrinterModel>();
                await Database.StoreAsync(user);
                await Database.SaveChangesAsync();
            }
        }
    }
}
