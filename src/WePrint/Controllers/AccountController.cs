using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.ApplicationInsights.Extensibility.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using WePrint.Controllers.Base;
using WePrint.Data;
using WePrint.Models;
using WePrint.Models.Authentication;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/account")]
    public class AccountController : WePrintController
    {
        private readonly SignInManager<User> _signInManager;
        private readonly UserManager<User> _userManager;
        private readonly IUserStore<User> _userStore;

        public AccountController(
            SignInManager<User> signInManager, 
            UserManager<User> userManager, 
            IUserStore<User> userStore,
            IServiceProvider services) : base(services)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _userStore = userStore;
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

        [HttpGet("personaldata")]
        public async Task<IActionResult> GetPersonalData()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            Log.LogInformation("User with ID '{UserId}' asked for their personal data.", _userManager.GetUserId(User));

            // Only include personal data for download
            var personalData = new Dictionary<string, string>();
            var personalDataProps = typeof(User).GetProperties()
                .Where(prop => Attribute.IsDefined(prop, typeof(PersonalDataAttribute)));

            foreach (var p in personalDataProps)
            {
                personalData.Add(p.Name, p.GetValue(user)?.ToString() ?? "null");
            }

            var logins = await _userManager.GetLoginsAsync(user);
            foreach (var l in logins)
            {
                personalData.Add($"{l.LoginProvider} external login provider key", l.ProviderKey);
            }

            personalData.Add($"Authenticator Key", await _userManager.GetAuthenticatorKeyAsync(user));

            Response.Headers.Add("Content-Disposition", "attachment; filename=PersonalData.json");
            return new FileContentResult(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(personalData, Formatting.Indented)), "application/json");
        }

        [HttpDelete("personaldata")]
        public async Task<IActionResult> DeletePersonalData([FromBody] DeletePersonalDataModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var RequirePassword = await _userManager.HasPasswordAsync(user);
            if (RequirePassword)
            {
                if (!await _userManager.CheckPasswordAsync(user, model.Password))
                {
                    return Forbid("Incorrect password.");
                }
            }

            user.Deleted = true;
            await _userManager.UpdateAsync(user);
            
            var userId = await _userManager.GetUserIdAsync(user);
            await _signInManager.SignOutAsync();

            Log.LogInformation("User with ID '{UserId}' deleted themselves.", userId);

            return Redirect("~/");
        }

        [HttpGet("externallogins")]
        public async Task<IActionResult> GetExternalLogins()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var currentLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync())
                .Where(auth => currentLogins.All(ul => auth.Name != ul.LoginProvider))
                .ToList();

            string passwordHash = null;
            if (_userStore is IUserPasswordStore<User> userPasswordStore)
            {
                passwordHash = await userPasswordStore.GetPasswordHashAsync(user, HttpContext.RequestAborted);
            }

            var showRemoveButton = passwordHash != null || currentLogins.Count > 1;

            return Ok(new ExternalLoginsModel
            {
                CurrentLogins = currentLogins,
                OtherLogins = otherLogins,
                ShowRemoveButton = showRemoveButton
            });
        }

        [HttpDelete("externallogins/{provider}")]
        public async Task<IActionResult> RemoveExternalLogin(string provider, string key)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var result = await _userManager.RemoveLoginAsync(user, provider, key);
            if (!result.Succeeded)
            {
                return Problem("The external login was not removed.");
            }

            await _signInManager.RefreshSignInAsync(user);
            return Problem("The external login was removed.");
        }

        [HttpPost("externallogins/{provider}")]
        public async Task<IActionResult> AddExternalLogin(string provider)
        {
            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);

            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LoginCallback");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [HttpGet("externallogins/callback")]
        public async Task<IActionResult> LoginCallback()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            var userId = await _userManager.GetUserIdAsync(user);
            var info = await _signInManager.GetExternalLoginInfoAsync(userId);
            if (info == null)
            {
                throw new InvalidOperationException($"Unexpected error occurred loading external login info for user with ID '{userId}'.");
            }

            var result = await _userManager.AddLoginAsync(user, info);
            if (!result.Succeeded)
            {
                return Problem("The external login was not added.External logins can only be associated with one account.");
            }

            // Clear the existing external cookie to ensure a clean login process
            await HttpContext.SignOutAsync(IdentityConstants.ExternalScheme);
            return Ok("The external login was added.");
        }
    }
}
