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
using WePrint.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/printer")]
    public class PrinterController : Controller
    {
        private readonly ILogger<PrinterController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public PrinterController(ILogger<PrinterController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }


        // GET: /api/Printer/
        [HttpGet]
        public async Task<IActionResult> GetPrinters()
        {
            var user = await this.GetCurrentUser(_session);
            if (user == null) return Unauthorized();

            return Json(user.Printers);
        }

        // GET" /api/Printer/id
        [HttpGet("{id}")]
        public Task<IActionResult> GetPrinterByID([FromRoute]string id)
            => this.QueryItemById<PrinterModel>(_session, id);

        // POST: /api/Printer/
        [HttpPost]
        public async Task<IActionResult> CreatePrinter([FromBody]PrinterModel model)
        {
            var user = await this.GetCurrentUser(_session);
            if (user.Printers == null)
                user.Printers = new List<PrinterModel>();
            user.Printers.Add(model);
            try
            {
                await _session.StoreAsync(model);
                // Don't think this works: For some reason, even after this call, user.Printers is always null
                await _session.StoreAsync(user);
            }
            catch(Exception e)
            {
                return this.FailWith(new { err = "Uncaught Exception", details = e.ToString() });
            }
            return Ok(model.Id);
        }

        // PUT: /api/Printer/
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePrinter([FromRoute]string id, [FromBody] PrinterUpdateModel model)
        {
            var user = await this.GetCurrentUser(_session);

            if (user == null) 
                return Unauthorized();

            var printer = user.Printers.FirstOrDefault(p => p.Id == id);

            if (printer == null)
                return NotFound("Printer " + id + " not found");

            ReflectionHelper.CopyPropertiesTo(model, printer);

            try
            {
                await _session.StoreAsync(user);
            }
            catch (Exception e)
            {
                return this.FailWith(new { err = "Uncaught Exception", details = e.ToString() });
            }
            return Ok();
        }

        // DELETE: /api/Printer/
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePrinter([FromRoute] string id)
        {
            var user = await this.GetCurrentUser(_session);

            if (user == null)
                return Unauthorized();

            var printer = user.Printers.FindIndex(p => p.Id == id);

            if (printer == -1)
                return NotFound("Printer " + id + " not found");

            try
            {
                user.Printers.RemoveAt(printer);
                await _session.StoreAsync(user);
            }
            catch (Exception e)
            {
                return this.FailWith(new { err = "Uncaught Exception", details = e.ToString() });
            }
            return Ok();
        }
    }
}
