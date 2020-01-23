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
    [Route("api/[controller]")]
    public class TestMessageController : Controller
    {
        private readonly ILogger<TestMessageController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public TestMessageController(ILogger<TestMessageController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }

        // GET: /api/Home
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            return Json(await _session.Query<TestMessage>().ToListAsync());
        }

        // POST: /api/Home
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]TestMessage m)
        {
            if (ModelState.IsValid)
            {
                m.Timestamp = DateTimeOffset.Now;
                m.Id = null;
                await _session.StoreAsync(m);
                await _session.SaveChangesAsync();
                return Ok();
            }
            return StatusCode(400);
        }

        // GET: /api/Home/{id}
        [HttpGet("{id}")]
        public async Task<IActionResult> Detail([FromRoute]string id)
        {
            var message = await _session.LoadAsync<TestMessage>(HttpUtility.UrlDecode(id));
            if (message == null)
                return NotFound();
            return Json(message);
        }
    }
}
