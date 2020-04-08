using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePrint.Data;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {
        public UserController(ILogger<UserController> log, UserManager<User> userManager, WePrintContext database) : base(log, userManager, database)
        {
        }

        // GET: /api/user/
        [HttpGet]
        public async Task<ActionResult<User>> GetCurrentUser()
        {
            var user = await CurrentUser;
            if (user == null)
                return Unauthorized();

            return Ok(user);
        }

        // GET" /api/user/id
        [HttpGet("{id}")]
        public async Task<ActionResult<User>> GetUserById([FromRoute]string id)
        {
            var targetUser = await Database.Users.FindAsync(id);
            if (targetUser == null) return NotFound();

            return Ok(targetUser);
        }
    }
}
