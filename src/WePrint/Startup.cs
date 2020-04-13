using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.EquivalencyExpression;
using ClacksMiddleware.Extensions;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Azure;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using WePrint.Data;
using WePrint.Models;
using WePrint.Permissions;

namespace WePrint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<WePrintContext>(options =>
                options.UseSqlServer(
                    Configuration.GetConnectionString("WePrint")));

            services.AddDefaultIdentity<User>()
                .AddEntityFrameworkStores<WePrintContext>();

            services.AddAuthentication();

            services.AddControllersWithViews().AddNewtonsoftJson(x =>
            {
                x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            services.AddRazorPages();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "WePrint", Version = "v1"});
                c.EnableAnnotations();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration => { configuration.RootPath = "ClientApp/build"; });

            services.AddDataProtection()
                .PersistKeysToFileSystem(new DirectoryInfo("./Secrets"))
                .SetApplicationName("WePrint");

            services.ConfigureApplicationCookie(options =>
            {
                options.Cookie.Name = "WePrint.Auth";
                options.Events = new CookieAuthenticationEvents()
                {
                    OnRedirectToLogin = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 401;
                        }

                        return Task.CompletedTask;
                    },
                    OnRedirectToAccessDenied = (ctx) =>
                    {
                        if (ctx.Request.Path.StartsWithSegments("/api") && ctx.Response.StatusCode == 200)
                        {
                            ctx.Response.StatusCode = 403;
                        }

                        return Task.CompletedTask;
                    }
                };
            });

            services.AddAutoMapper((serviceProvider, mapper) =>
                {
                    mapper.AddCollectionMappers();
                    
                    // Data/View/Create
                    mapper.AddProfile<AutoProfile<Project, ProjectViewModel, ProjectCreateModel, Guid>>();
                    mapper.AddProfile<AutoProfile<ProjectUpdate, ProjectUpdateViewModel, ProjectUpdateCreateModel, Guid>>();
                    mapper.AddProfile<AutoProfile<Pledge, PledgeViewModel, PledgeCreateModel, Guid>>();
                    mapper.AddProfile<AutoProfile<Organization, OrganizationViewModel, OrganizationCreateModel, Guid>>();
                    mapper.AddProfile<AutoProfile<Job, JobViewModel, JobCreateModel, Guid>>();

                    // Data/View
                    mapper.AddProfile<AutoProfile<User, UserViewModel, Guid>>();
                    mapper.AddProfile<AutoProfile<Printer, PrinterViewModel, Guid>>();
                }, 
                // Auto-Finding profiles has been disabled, because it kept trying to add the AutoProfile.
                // You'll need to add new profiles here 
                new Assembly[0]);

            services.RegisterPermissions();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.GnuTerryPratchett();

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    "default",
                    "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1"); });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment()) spa.UseReactDevelopmentServer("start");
            });
        }
    }
}