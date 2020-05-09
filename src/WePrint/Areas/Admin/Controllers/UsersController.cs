using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using WePrint.Data;
using WePrint.Models;

namespace WePrint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class UsersController : ManagementController<User>
    {
        private readonly RoleManager<Role> _roleManager;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IHostEnvironment _hostEnvironment;

        public UsersController(
            RoleManager<Role> roleManager, 
            UserManager<User> userManager,
            SignInManager<User> signInManager, 
            IHostEnvironment hostEnvironment,
            IServiceProvider services) : base(services)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _signInManager = signInManager;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> Promote(Guid id)
        {
            if (!await _roleManager.RoleExistsAsync("Administrator"))
            {
                var role = new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Administrator"
                };
                await _roleManager.CreateAsync(role);
            }

            var user = await _userManager.GetUserAsync(User);
            await _userManager.AddToRoleAsync(user, "Administrator");

            return RedirectToAction("Details", new {id});
        }

        public async Task<IActionResult> Demote(Guid id)
        {
            if (!await _roleManager.RoleExistsAsync("Administrator"))
            {
                var role = new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Administrator"
                };
                await _roleManager.CreateAsync(role);
            }

            var user = await _userManager.GetUserAsync(User);
            await _userManager.RemoveFromRoleAsync(user, "Administrator");
            
            return RedirectToAction("Details", new { id });
        }

        // This is an *extremely* dangerous method, and should not even be built into a Release binary
#if DEBUG
        public async Task<IActionResult> Impersonate(Guid id)
        {
            // Second check to make sure this cannot be used in prod
            if (_hostEnvironment.IsProduction())
                return BadRequest();

            var user = await _userManager.FindByIdAsync(id.ToString());
            if (user == null)
                return NotFound();
            
            await _signInManager.SignInAsync(user, false);
            return RedirectToAction("Details", new {id});
        }
#else
        public async Task<IActionResult> Impersonate(Guid id)
        {
            return BadRequest();
        }
#endif
    }
}
