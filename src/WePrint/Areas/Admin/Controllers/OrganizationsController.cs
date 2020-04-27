using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WePrint.Areas.Admin.Models;
using WePrint.Data;
using WePrint.Models;
using WePrint.Utilities;

namespace WePrint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class OrganizationsController : ManagementController<Organization>
    {
        private readonly WePrintContext _context;
        private readonly IAvatarProvider _avatar;

        public OrganizationsController(WePrintContext context, IAvatarProvider avatar, IServiceProvider services) : base(services)
        {
            _context = context;
            _avatar = avatar;
        }

        [HttpPost]
        public async Task<IActionResult> CreateOrg(Organization org, [FromForm(Name = "Avatar")] IFormFile avatar)
        {
            if (!ModelState.IsValid)
            {
                return View("Create", org);
            }

            _context.Organizations.Add(org);
            await _context.SaveChangesAsync();

            if (avatar != null)
                await _avatar.SetAvatarResult(org, avatar);

            return RedirectToAction("Details", new {id = org.Id});
        }

        [HttpPost]
        public async Task<IActionResult> EditOrg(Organization org, [FromForm(Name = "Avatar")] IFormFile avatar)
        {
            ModelState.Remove("Avatar");
            if (!ModelState.IsValid)
            {
                return View("Edit", org);
            }

            _context.Organizations.Update(org);
            await _context.SaveChangesAsync();

            if (avatar != null)
                await _avatar.SetAvatarResult(org, avatar);

            return RedirectToAction("Details", new { id = org.Id });
        }
    }
}