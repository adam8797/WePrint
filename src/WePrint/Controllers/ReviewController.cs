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
    [Route("api/review")]
    public class ReviewController : Controller
    {
        private readonly ILogger<ReviewController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public ReviewController(ILogger<ReviewController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }


        // GET: /api/Review/
        [HttpGet]
        public async Task<IActionResult> GetReviews()
        {
            return Json("Under Construction");
        }

        // GET" /api/Review/id
        [HttpGet("{id}")]
        public Task<IActionResult> GetReviewByID([FromRoute]string id)
            => this.QueryItemById<ReviewModel>(_session, id);

        // POST: /api/Review/
        [HttpPost]
        public async Task<IActionResult> CreateReview()
        {
            return Json("Under construction");
        }

        // PUT: /api/Review/
        [HttpPut]
        public async Task<IActionResult> UpdateReview()
        {
            return Json("Under construction");
        }

        // DELETE: /api/Review/
        [HttpDelete]
        public async Task<IActionResult> DeleteReview()
        {
            return Json("Under construction");
        }
    }
}
