using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Linq;
using Raven.Client.Documents.Session;
using WePrint.Common;
using WePrint.Common.ServiceDiscovery;
using WePrint.Common.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/device")]
    public class DeviceController : WePrintController
    {
        public DeviceController(ILogger<DeviceController> log, UserManager<ApplicationUser> userManager, IAsyncDocumentSession database) : base(log, userManager, database)
        {
        }

        // GET: /api/Device/
        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            var user = await CurrentUser;
            if (user == null) 
                return Unauthorized();

            user.Printers ??= new List<PrinterModel>();
            
            return Ok(user.Printers);
        }

        // GET" /api/Device/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceById([FromRoute]string id)
        {
            var user = await CurrentUser;
            if (user == null) 
                return Unauthorized();

            user.Printers ??= new List<PrinterModel>();

            var printer = user.Printers.FirstOrDefault(x => x.Id == id);
            if (printer == null) return NotFound();

            return Ok(printer);
        }

        // POST: /api/Device/
        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody]PrinterModel model)
        {
            var user = await CurrentUser;

            if (user == null)
                return Unauthorized();

            user.Printers ??= new List<PrinterModel>();
            model.Id = Guid.NewGuid().ToString();
            user.Printers.Add(model);

            await Database.SaveChangesAsync();

            return Created(Url.Action("GetDeviceByID", new {id = model.Id}), model);
        }

        // PUT: /api/Device/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice([FromRoute]string id, [FromBody] PrinterUpdateModel model)
        {
            var user = await CurrentUser;

            if (user == null)
                return Unauthorized();
            
            user.Printers ??= new List<PrinterModel>();

            var device = user.Printers.FirstOrDefault(p => p.Id == id);

            if (device == null)
                return NotFound("Device " + id + " not found");

            ReflectionHelper.CopyPropertiesTo(model, device);

            await Database.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /api/Device/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice([FromRoute] string id)
        {
            var user = await CurrentUser;

            if (user == null)
                return Unauthorized();

            user.Printers ??= new List<PrinterModel>();

            var device = user.Printers.FirstOrDefault(p => p.Id == id);

            if (device == null)
                return NotFound("Device " + id + " not found");

            user.Printers.Remove(device);

            await Database.SaveChangesAsync();
            return NoContent();
        }
    }
}
