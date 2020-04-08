﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
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
        protected readonly IMapper Mapper;

        protected readonly AsyncLazy<User> CurrentUser;

        protected ControllerBase(IServiceProvider services)
        {
            Log = (ILogger)services.GetRequiredService(typeof(ILogger<>).MakeGenericType(GetType()));
            UserManager = services.GetRequiredService<UserManager<User>>();
            Database = services.GetRequiredService<WePrintContext>();
            Mapper = services.GetRequiredService<IMapper>();
            CurrentUser = new AsyncLazy<User>(async () => await UserManager.GetUserAsync(HttpContext.User));
        }
    }
}
