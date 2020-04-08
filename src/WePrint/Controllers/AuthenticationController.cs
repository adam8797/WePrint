using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using WePrint.Data;
using WePrint.Models;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthenticationController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;

        public AuthenticationController(
            ILogger<AuthenticationController> log,
            WePrintContext dbSession,
            UserManager<User> userManager,
            SignInManager<User> signInManager)
            : base(log, userManager, dbSession)
        {
            _signInManager = signInManager;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterModel model)
        {
            var user = new User()
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                UserName = model.Username,
                Email = model.Email
            };

            var result = await UserManager.CreateAsync(user, model.Password);
            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await _signInManager.SignInAsync(user, true);
            return Ok();
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LogInModel login)
        {
            var result = await _signInManager.PasswordSignInAsync(login.Username, login.Password, login.Remember, false);
            
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
        public async Task<IActionResult> ChangePassword(PasswordChangeModel model)
        {
            var currentUser = await CurrentUser;

            var result = await UserManager.ChangePasswordAsync(currentUser, model.OldPassword, model.NewPassword);

            if (result.Succeeded)
                return Ok();

            return BadRequest(result.Errors);
        }

        [HttpGet("logout")]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }
    }
}
