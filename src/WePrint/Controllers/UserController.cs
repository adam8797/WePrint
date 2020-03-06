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
using WePrint.Common.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : WePrintController
    {
        public UserController(ILogger log, IAsyncDocumentSession database) : base(log, database)
        {
        }

        // GET: /api/user/
        [HttpGet]
        public async Task<IActionResult> GetCurrentUser()
        {
            var user = await CurrentUser;
            if (user == null) return Unauthorized();

            return Json(user.GetPublicModel());
        }

        // GET" /api/user/id
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserById([FromRoute]string id)
        {
            var user = await CurrentUser;
            if (user == null) return Unauthorized();

            var tgtUser = await Database.LoadAsync<ApplicationUser>(id);
            if (tgtUser == null) return NotFound();

            return Json(tgtUser.GetPublicModel());
        }
    }
}
