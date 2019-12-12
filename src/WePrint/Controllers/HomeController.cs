using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePrint.Common.ServiceDiscovery;
using WePrint.Common.ServiceDiscovery.Services;
using WePrint.Models;

namespace WePrint.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IServiceDiscovery _discovery;

        public HomeController(ILogger<HomeController> logger, IServiceDiscovery discovery)
        {
            _logger = logger;
            _discovery = discovery;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> ListRaven()
        {
            var config = await _discovery.DiscoverAsync<RavenDBDiscoveredService>();
            return View(config.Hosts);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
