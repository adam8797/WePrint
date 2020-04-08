using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Nito.AsyncEx;
using WePrint.Data;

namespace WePrint.Controllers
{
    public abstract class ControllerBase : Controller
    {
        protected readonly ILogger Log;
        protected readonly UserManager<User> UserManager;
        protected readonly WePrintContext Database;

        protected readonly AsyncLazy<User> CurrentUser;

        protected ControllerBase(ILogger log, UserManager<User> userManager, WePrintContext database)
        {
            Database = database;
            Log = log;
            UserManager = userManager;
            CurrentUser = new AsyncLazy<User>(async () => await UserManager.GetUserAsync(HttpContext.User));
        }
    }
}
