using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using WePrint.Controllers.Base;
using WePrint.Data;
using WePrint.Models;

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/devices")]
    public class device_controller : we_print_rest_controller<printer, printer_view_model, printer_create_model, Guid>
    {
        public device_controller(IServiceProvider services) : base(services)
        {
        }

        protected override IQueryable<printer> filter(IQueryable<printer> data, user user)
        {
            return data.Where(x => x.owner == user);
        }

        protected override async ValueTask<printer> create_data_model_async(printer_create_model view_model)
        {
            var printer = mapper.Map<printer>(view_model);
            printer.owner = await current_user;
            return printer;
        }
    }
}
