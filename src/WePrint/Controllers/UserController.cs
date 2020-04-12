using System;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePrint.Controllers.Base;
using WePrint.Models.User;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/users")]
    public class UserController : WePrintController
    {
        public UserController(IServiceProvider services) : base(services)
        {
        }

        // GET: /api/users/
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<UserViewModel>> GetCurrentUser()
        {
            var vm = Mapper.Map<UserViewModel>(await CurrentUser);
            return Ok(vm);
        }

        // GET" /api/users/by-id/{id}
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

        // GET" /api/users/by-name/{id}
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
