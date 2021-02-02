using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using CMS.Models;
using Microsoft.Extensions.Hosting;

namespace CMS
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
            /*Identity*/
            services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(Configuration["ConnectionStrings:IdentityConnection"]));
            services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
            /*End*/

            /*Identity Login Url */
            services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Login");

            services.AddControllersWithViews();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            /*Identity*/
            app.UseAuthentication();
            app.UseAuthorization();
            /*End*/

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "View-Blog",
                    pattern: "b/{name}",
                    defaults: new { controller = "Home", action = "ViewBlog" });

                endpoints.MapControllerRoute(
                    name: "PagingPageOne",
                    pattern: "{controller}",
                    defaults: new { controller = "Home", action = "Index", id = 1 });

                endpoints.MapControllerRoute(
                    name: "Paging",
                    pattern: "{controller}/{id:int?}",
                    defaults: new { controller = "Home", action = "Index" });

                endpoints.MapControllerRoute(
                    name: "MatchUrl",
                    pattern: "{name:required}",
                    defaults: new { controller = "Home", action = "Page" });

                endpoints.MapControllerRoute(
                   name: "default",
                   pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
