using System;
using System.IO;
using System.Text.RegularExpressions;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using Nito.AsyncEx;
using WePrint.Data;
using WePrint.Models;
using WePrint.Utilities;

namespace WePrint.Controllers.Base
{
    public abstract class WePrintController : ControllerBase
    {
        protected readonly ILogger Log;
        protected readonly UserManager<User> UserManager;
        protected readonly WePrintContext Database;
        protected readonly IMapper Mapper;
        protected readonly IConfiguration Configuration;
        protected readonly AsyncLazy<User> CurrentUser;
        protected readonly IBlobContainerProvider BlobContainerProvider;

        protected WePrintController(IServiceProvider services)
        {
            Log = (ILogger) services.GetRequiredService(typeof(ILogger<>).MakeGenericType(GetType()));
            UserManager = services.GetRequiredService<UserManager<User>>();
            Database = services.GetRequiredService<WePrintContext>();
            Mapper = services.GetRequiredService<IMapper>();
            Configuration = services.GetRequiredService<IConfiguration>();
            CurrentUser = new AsyncLazy<User>(async () => await UserManager.GetUserAsync(HttpContext.User));
            BlobContainerProvider = services.GetRequiredService<IBlobContainerProvider>();
        }
    }
}
