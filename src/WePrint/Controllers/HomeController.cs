using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using WePrint.Common.ServiceDiscovery;
using WePrint.Common.ServiceDiscovery.Services;
using WePrint.Models;

namespace WePrint.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public HomeController(ILogger<HomeController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }

        public async Task<IActionResult> Index()
        {
            return View(await _session.Query<TestMessage>().ToListAsync());
        }

        [HttpPost]
        public async Task<IActionResult> Index(TestMessage m)
        {
            if (ModelState.IsValid)
            {
                m.Timestamp = DateTimeOffset.Now;
                await _session.StoreAsync(m);
                await _session.SaveChangesAsync();
                return await Index();
            }
            return StatusCode(400);
        }

        public async Task<IActionResult> Detail(string id)
        {
            var message = await _session.LoadAsync<TestMessage>(HttpUtility.UrlDecode(id));
            if (message == null)
                return NotFound();
            return View(message);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
