using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using WePrint.Controllers.Base;
using WePrint.Data;
using AutoMapper.QueryableExtensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using WePrint.Models;
using WePrint.Utilities;

namespace WePrint.Controllers
{
    [ApiController]
    [Route("api/projects")]
    [Authorize]
    public class ProjectController : WePrintFileRestController<Project, ProjectViewModel, ProjectCreateModel, Guid>
    {
        public ProjectController(IServiceProvider services) : base(services)
        {
        }

        #region REST Implementation

        protected override async ValueTask<Project> CreateDataModelAsync(ProjectCreateModel viewModel)
        {
            var p = Mapper.Map<Project>(viewModel);
            var user = await CurrentUser;
            if (user.Organization == null)
                throw new InvalidOperationException($"Cannot create a project if user {user.UserName} is not a member of an organization");
            p.Organization = user.Organization;
            return p;
        }

        #endregion
    }
}