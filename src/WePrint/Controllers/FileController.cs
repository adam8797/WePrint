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
    [Route("api/file")]
    public class FileController : Controller
    {
        private readonly ILogger<FileController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public FileController(ILogger<FileController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }


        // GET: /api/File/
        [HttpGet]
        public async Task<IActionResult> GetFiles()
        {
            return Json("Under Construction");
        }

        // GET" /api/File/id
        [HttpGet("{id}")]
        public Task<IActionResult> GetFileByID([FromRoute]string id)
            => this.QueryItemById<FileModel>(_session, id);

        // POST: /api/File/
        [HttpPost]
        public async Task<IActionResult> CreateFile()
        {
            return Json("Under construction");
        }

        // PUT: /api/File/
        [HttpPut]
        public async Task<IActionResult> UpdateFile()
        {
            return Json("Under construction");
        }

        // DELETE: /api/File/
        [HttpDelete]
        public async Task<IActionResult> DeleteFile()
        {
            return Json("Under construction");
        }
    }
}
