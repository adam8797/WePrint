using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Web;
using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using Raven.Client.Documents.Linq;
using WePrint.Common.ServiceDiscovery;
using WePrint.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : Controller
    {
        private readonly ILogger<UserController> _logger;
        private readonly IServiceDiscovery _discovery;
        private readonly IAsyncDocumentSession _session;

        public UserController(ILogger<UserController> logger, IServiceDiscovery discovery, IAsyncDocumentSession session)
        {
            _logger = logger;
            _session = session;
            _discovery = discovery;
        }


        // GET: /api/user/
        [HttpGet]
        public async Task<IActionResult> GetUsers() 
        {
            var user = await this.GetCurrentUser(_session);
            if (user == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            var allUsers = await _session.Query<ApplicationUser>().Select(u => u.GetPublicUser()).ToArrayAsync();
            return Json(allUsers);
        }

        // TODO probably need to authenticate this?
        // GET" /api/user/id
        [HttpGet("{id}")]
        public Task<IActionResult> GetUserByID([FromRoute]string id)
            => this.QueryItemById<ApplicationUser>(_session, id);

        // PUT: /api/user/
        [HttpPut]
        public async Task<IActionResult> UpdateUser([FromBody]ApplicationUserUpdateModel update)
        {
            var user = await this.GetCurrentUser(_session);
            if (user == null)
                return this.FailWith("Not logged in", HttpStatusCode.Unauthorized);

            if (user.Id != update.Id)
                return this.FailWith("Current user is not authorized for this user", HttpStatusCode.Unauthorized);

            user.ApplyChanges(update);

            try
            {
                await _session.StoreAsync(user);
                await _session.SaveChangesAsync();
            }
            catch (Exception e)
            {
                return this.FailWith(new { err = "Uncaught Exception", details = e.ToString() }, HttpStatusCode.InternalServerError);
            }

            return Json(user.Id);
        }

        // DELETE: /api/user/
        [HttpDelete]
        public async Task<IActionResult> DeleteUser()
        {
            return Json("Under construction");
        }
    }
}
