using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePrint.Data;
using WePrint.ViewModels;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/user")]
    public class UserController : ControllerBase
    {

        public UserController(IServiceProvider services) : base(services)
        {
        }

        // GET: /api/user/
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserViewModel>> GetCurrentUser()
        {
            var vm = Mapper.Map<UserViewModel>(await CurrentUser);
            return Ok(vm);
        }

        // GET" /api/user/by-id/{id}
        [HttpGet("by-id/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserViewModel>> GetUserById([FromRoute]string id)
        {
            var targetUser = await UserManager.FindByIdAsync(id);
            if (targetUser == null)
                return NotFound();

            var vm = Mapper.Map<UserViewModel>(targetUser);
            return vm;
        }

        // GET" /api/user/by-name/{id}
        [HttpGet("by-name/{id}")]
        [AllowAnonymous]
        public async Task<ActionResult<UserViewModel>> GetUserByUsername([FromRoute]string id)
        {
            var targetUser = await UserManager.FindByNameAsync(id);
            if (targetUser == null)
                return NotFound();

            var vm = Mapper.Map<UserViewModel>(targetUser);
            return vm;
        }
    }
}
