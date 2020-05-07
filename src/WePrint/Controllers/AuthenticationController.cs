using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using WePrint.Controllers.Base;
using WePrint.Data;
using WePrint.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class authentication_controller : we_print_controller
    {
        private readonly SignInManager<user> _sign_in_manager;

        public authentication_controller(SignInManager<user> sign_in_manager, IServiceProvider services) : base(services)
        {
            _sign_in_manager = sign_in_manager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> register(register_model model)
        {
            var user = new user()
            {
                first_name = model.first_name,
                last_name = model.last_name,
                UserName = model.username,
                Email = model.email
            };

            var result = await user_manager.CreateAsync(user, model.password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _sign_in_manager.SignInAsync(user, true);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> login(log_in_model login)
        {
            var result = await _sign_in_manager.PasswordSignInAsync(login.username, login.password, login.remember, false);
            
            if (result.IsLockedOut)
                return Unauthorized("Account is locked");

            if (result.IsNotAllowed)
                return Unauthorized("Account is locked");

            if (result.RequiresTwoFactor)
                return Unauthorized("Two Factor Auth Needed");

            if (result.Succeeded)
                return Ok();

            return Unauthorized();
        }

        [HttpPost("changePassword")]
        [Authorize]
        public async Task<IActionResult> change_password(password_change_model model)
        {
            var current_user = await base.current_user;

            var result = await user_manager.ChangePasswordAsync(current_user, model.old_password, model.new_password);

            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> logout()
        {
            await _sign_in_manager.SignOutAsync();
            return Ok();
        }
    }
}
