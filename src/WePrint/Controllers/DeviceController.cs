using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePrint.Data;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/device")]
    [Authorize]
    public class DeviceController : ControllerBase
    {
        public DeviceController(ILogger<DeviceController> log, UserManager<User> userManager, WePrintContext database) : base(log, userManager, database)
        {
        }

        // GET: /api/Device/
        [HttpGet]
        public async Task<IActionResult> GetDevices()
        {
            var user = await CurrentUser;
            return Ok(user.Printers);
        }

        // GET" /api/Device/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetDeviceById([FromRoute]Guid id)
        {
            var user = await CurrentUser;
            var printer = user.Printers.FirstOrDefault(x => x.Id == id);
            if (printer == null) return NotFound();

            return Ok(printer);
        }

        // POST: /api/Device/
        [HttpPost]
        public async Task<IActionResult> CreateDevice([FromBody]Printer model)
        {
            var user = await CurrentUser;

            user.Printers.Add(model);

            await Database.SaveChangesAsync();

            return CreatedAtAction("GetDeviceByID", new {id = model.Id}, model);
        }

        // PUT: /api/Device/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateDevice([FromRoute]Guid id, [FromBody] Printer model)
        {
            var user = await CurrentUser;
            
            var device = user.Printers.FirstOrDefault(p => p.Id == id);

            if (device == null)
                return NotFound("Device " + id + " not found");

            ReflectionHelper.Update(model, device);

            await Database.SaveChangesAsync();

            return NoContent();
        }

        // DELETE: /api/Device/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteDevice([FromRoute] Guid id)
        {
            var user = await CurrentUser;

            var device = user.Printers.FirstOrDefault(p => p.Id == id);

            if (device == null)
                return NotFound("Device " + id + " not found");

            user.Printers.Remove(device);

            await Database.SaveChangesAsync();
            return NoContent();
        }
    }
}
