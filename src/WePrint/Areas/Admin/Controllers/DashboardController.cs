using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WePrint.Data;
using WePrint.Models;

namespace WePrint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize]
    public class DashboardController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly SignInManager<User> _signInManager;

        public DashboardController(UserManager<User> userManager, RoleManager<Role> roleManager, SignInManager<User> signInManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _signInManager = signInManager;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Promote()
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
            
            // Need to refresh the user claims, so re-sign in
            await _signInManager.SignInAsync(user, true);

            return RedirectToAction("Index");
        }
    }
}