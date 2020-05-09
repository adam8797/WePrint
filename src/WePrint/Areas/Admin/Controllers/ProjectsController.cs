using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WePrint.Models;

namespace WePrint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class ProjectsController : ManagementController<Project>
    {
        public ProjectsController(IServiceProvider services) : base(services)
        {
        }
    }
}
