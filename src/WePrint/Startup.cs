using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using EasyNetQ;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Raven.Client.Documents;
using Raven.DependencyInjection;
using WePrint.Common.ServiceDiscovery;
using Raven.Identity;
using WePrint.Common.Models;
using WePrint.Common.Slicer.Impl;
using WePrint.Common.Slicer.Interface;
using WePrint.Raven;
using IdentityRole = Raven.Identity.IdentityRole;

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
            services.AddScoped<IServiceDiscovery>(x => new DNSServiceDiscovery(Configuration));

            services.AddRavenDbDocStore(x =>
                {
                    var discovery = new DNSServiceDiscovery(Configuration);
                    discovery.DiscoverToAsync(x, "RavenDB",
                            (ravenOptions, urls) =>
                                ravenOptions.Settings.Urls = urls.Select(url => "http://" + url + ":8080").ToArray(),
                            (ravenOptions, config) => config.Bind(ravenOptions.Settings))
                        .Wait();

                    x.BeforeInitializeDocStore = store =>
                    {
                        store.Conventions.IdentityPartsSeparator = "-";
                    };
                })
                .AddRavenDbAsyncSession()
                .AddRavenDbSession();


            //ToDo: Load this from config
            services.RegisterEasyNetQ("host=localhost;username=user;password=password");
            if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Production")
                services.AddSingleton<ISlicerService>(x => new RabbitMQSlicerService(x.GetRequiredService<IBus>(), "Slicer", x.GetRequiredService<IDocumentStore>()));
            else
                services.AddSingleton<ISlicerService>(x => new RabbitMQSlicerService(x.GetRequiredService<IBus>(), "Slicer_Testing", x.GetRequiredService<IDocumentStore>()));

            services.AddSingleton<RavenPersistedGrantStore>();

            services.AddIdentity<ApplicationUser, IdentityRole>()
                .AddRavenDbIdentityStores<ApplicationUser>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddIdentityServer()
                .AddIdentityResources()
                .AddPersistedGrantStore<RavenPersistedGrantStore>()
                .AddApiResources()
                .AddClients()
                .AddSigningCredentials()
                .AddAspNetIdentity<ApplicationUser>();

            services.AddAuthentication()
                .AddIdentityServerJwt();

            services.AddControllersWithViews()
                .AddNewtonsoftJson();

            services.AddRazorPages();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "WePrint", Version = "v1" });
                c.EnableAnnotations();

                // Set the comments path for the Swagger JSON and UI.
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            // In production, the React files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/build";
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDocumentStore store)
        {
            store.SetupApplicationDependencies();

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

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseIdentityServer();
            app.UseAuthorization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseReactDevelopmentServer(npmScript: "start");
                }
            });
        }
    }
}
