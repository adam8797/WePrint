using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.UI;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.EntityFrameworkCore;
using WePrint.Data;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Raven.DependencyInjection;
using Raven.Identity;
using WePrint.Common.ServiceDiscovery;
using WePrint.Common.ServiceDiscovery.Services;
using IdentityRole = Raven.Identity.IdentityRole;
using Raven.Client.Documents;

namespace WePrint
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public static string AppVersion => Assembly.GetAssembly(typeof(Startup)).GetName().Version.ToString(4);

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
                    Debug.Print("Raven Configured");
                    foreach (var settingsUrl in x.Settings.Urls)
                    {
                        Debug.Print("  " + settingsUrl);
                    }
                })
                .AddRavenDbAsyncSession()
                .AddRavenDbSession();

            services
                .AddIdentity<WePrintUser, IdentityRole>()
                .AddRavenDbIdentityStores<WePrintUser>()
                .AddDefaultTokenProviders()
                .AddDefaultUI();

            services.AddControllersWithViews();

            services.AddRazorPages();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IDocumentStore docStore)
        {
            if (env.IsDevelopment() || env.IsTesting())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
                docStore.EnsureExists();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
                endpoints.MapRazorPages();
            });


        }
    }
}
