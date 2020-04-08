using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WePrint.Data;
using WePrint.ViewModels;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/device")]
    [Authorize]
    public class DeviceController : ControllerBase
    {

        public DeviceController(IServiceProvider services) : base(services)
        {
        }


        // GET: /api/Device/
        [HttpGet]
        public async Task<List<PrinterViewModel>> GetDevices()
        {
            var user = await CurrentUser;
            var devices = await Database.Printers.Where(x => x.Owner == user).ProjectTo<PrinterViewModel>(Mapper.ConfigurationProvider).ToListAsync();
            return devices;
        }

        // GET" /api/Device/id
        [HttpGet("{id}")]
        public async Task<ActionResult<PrinterViewModel>> GetDeviceById([FromRoute]Guid id)
        {
            var user = await CurrentUser;
            var printer = user.Printers.FirstOrDefault(x => x.Id == id);
            if (printer == null)
                return NotFound();

            return Mapper.Map<PrinterViewModel>(printer);
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
