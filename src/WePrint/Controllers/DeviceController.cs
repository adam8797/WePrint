using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WePrint.Controllers.Base;
using WePrint.Data;
using WePrint.Models;

namespace WePrint.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/devices")]
    public class DeviceController : WePrintRestController<Printer, PrinterViewModel, PrinterCreateModel, Guid>
    {
        public DeviceController(IServiceProvider services) : base(services)
        {
        }

        protected override IQueryable<Printer> Filter(IQueryable<Printer> data, User user)
        {
            return data.Where(x => x.Owner == user);
        }

        protected override async ValueTask<bool> AllowRead(User user, Printer entity)
        {
            return entity.Owner == user;
        }

        protected override DbSet<Printer> GetDbSet(WePrintContext database) => database.Printers;

        protected override async ValueTask<Printer> CreateDataModelAsync(PrinterCreateModel viewModel)
        {
            var printer = Mapper.Map<Printer>(viewModel);
            printer.Owner = await CurrentUser;
            return printer;
        }
    }
}
