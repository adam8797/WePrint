using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
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
            return Json("Under Construction");
        }

        // GET" /api/Printer/id
        [HttpGet("{id}")]
        public Task<IActionResult> GetPrinterByID([FromRoute]string id)
            => this.QueryItemById<PrinterModel>(_session, id);

        // POST: /api/Printer/
        [HttpPost]
        public async Task<IActionResult> CreatePrinter()
        {
            return Json("Under construction");
        }

        // PUT: /api/Printer/
        [HttpPut]
        public async Task<IActionResult> UpdatePrinter()
        {
            return Json("Under construction");
        }

        // DELETE: /api/Printer/
        [HttpDelete]
        public async Task<IActionResult> DeletePrinter()
        {
            return Json("Under construction");
        }
    }
}
