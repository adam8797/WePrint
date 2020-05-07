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
    public abstract class we_print_controller : ControllerBase
    {
        protected readonly ILogger log;
        protected readonly UserManager<user> user_manager;
        protected readonly WePrintContext database;
        protected readonly IMapper mapper;
        protected readonly IConfiguration configuration;
        protected readonly AsyncLazy<user> current_user;
        protected readonly IBlobContainerProvider blob_container_provider;

        protected we_print_controller(IServiceProvider services)
        {
            log = (ILogger) services.GetRequiredService(typeof(ILogger<>).MakeGenericType(GetType()));
            user_manager = services.GetRequiredService<UserManager<user>>();
            database = services.GetRequiredService<WePrintContext>();
            mapper = services.GetRequiredService<IMapper>();
            configuration = services.GetRequiredService<IConfiguration>();
            current_user = new AsyncLazy<user>(async () => await user_manager.GetUserAsync(HttpContext.User));
            blob_container_provider = services.GetRequiredService<IBlobContainerProvider>();
        }
    }
}
