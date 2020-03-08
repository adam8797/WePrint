using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using Raven.Client.Documents;
using Raven.Client.Documents.Session;
using WePrint.Common.Models;

namespace WePrint.Controllers
{
    public abstract class WePrintController : Controller
    {
        protected readonly IAsyncDocumentSession Database;
        protected readonly ILogger Log;
        protected readonly UserManager<ApplicationUser> UserManager;

        // This lazy may be unnecessary since Raven may do this for us, but I'm not sure
        protected readonly AsyncLazy<ApplicationUser> CurrentUser;

        protected WePrintController(ILogger log, UserManager<ApplicationUser> userManager, IAsyncDocumentSession database)
        {
            Database = database;
            Log = log;
            UserManager = userManager;
            CurrentUser = new AsyncLazy<ApplicationUser>(async () => await UserManager.GetUserAsync(HttpContext.User));
        }

        protected async Task<IEnumerable<ApplicationUser>> GetUsers()
        {
            return await Database.Query<ApplicationUser>().ToArrayAsync();
        }
    }
}
