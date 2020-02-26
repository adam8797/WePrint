using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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

        // This lazy may be unnecessary since Raven may do this for us, but I'm not sure
        protected readonly AsyncLazy<ApplicationUser> CurrentUser;

        protected WePrintController(ILogger log, IAsyncDocumentSession database)
        {
            Database = database;
            Log = log;
            CurrentUser = new AsyncLazy<ApplicationUser>(async () =>
            {
                var identity = HttpContext.User.Identity;

                if (!identity.IsAuthenticated)
                    return null;

                return await Database.Query<ApplicationUser>().SingleOrDefaultAsync(x => x.Email == identity.Name);
            });
        }
    }
}
