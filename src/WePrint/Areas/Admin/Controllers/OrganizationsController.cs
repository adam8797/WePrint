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

namespace WePrint.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Administrator")]
    public class OrganizationsController : ManagementController<Organization>
    {
        public OrganizationsController(IServiceProvider services) : base(services)
        {
        }
    }
}