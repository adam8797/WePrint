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
    public class dashboard_controller : Controller
    {
        private readonly UserManager<user> _user_manager;
        private readonly RoleManager<Role> _role_manager;
        private readonly SignInManager<user> _sign_in_manager;

        public dashboard_controller(UserManager<user> user_manager, RoleManager<Role> role_manager, SignInManager<user> sign_in_manager)
        {
            _user_manager = user_manager;
            _role_manager = role_manager;
            _sign_in_manager = sign_in_manager;
        }

        public IActionResult index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> promote()
        {
            if (!await _role_manager.RoleExistsAsync("Administrator"))
            {
                var role = new Role()
                {
                    Id = Guid.NewGuid(),
                    Name = "Administrator"
                };
                await _role_manager.CreateAsync(role);
            }

            var user = await _user_manager.GetUserAsync(User);
            await _user_manager.AddToRoleAsync(user, "Administrator");
            
            // Need to refresh the user claims, so re-sign in
            await _sign_in_manager.SignInAsync(user, true);

            return RedirectToAction("index");
        }
    }
}