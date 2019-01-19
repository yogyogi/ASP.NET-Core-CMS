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

            services.AddMvc();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseDeveloperExceptionPage();
                //app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();

            /*Identity*/
            app.UseAuthentication();
            /*End*/

            app.UseMvc(routes =>
            {
                /*Menu*/
                routes.MapRoute(
                name: "PagingPageOne-menu",
                template: "Menu",
                defaults: new { controller = "Menu", action = "Index", id = 1 }
                );

                routes.MapRoute(
                name: "Paging-menu",
                template: "Menu/{id:int?}",
                defaults: new { controller = "Menu", action = "Index" });
                /*End*/

                /*Page*/
                routes.MapRoute(
                name: "PagingPageOne-page",
                template: "Page",
                defaults: new { controller = "Page", action = "Index", id = 1 }
                );

                routes.MapRoute(
                name: "Paging-page",
                template: "Page/{id:int?}",
                defaults: new { controller = "Page", action = "Index" });
                /*End*/

                /*MyBlogCategory*/
                routes.MapRoute(
                name: "PagingPageOne-myblogcategory",
                template: "MyBlogCategory/{url}",
                defaults: new { controller = "Home", action = "MyBlogCategory", id = 1 }
                );

                routes.MapRoute(
                name: "Paging-myblogcategory",
                template: "MyBlogCategory/{url}/{id:int?}",
                defaults: new { controller = "Home", action = "MyBlogCategory" });
                /*End*/

                /*MyBlog*/
                routes.MapRoute(
                name: "PagingPageOne-myblog",
                template: "MyBlog",
                defaults: new { controller = "Home", action = "MyBlog", id = 1 }
                );

                routes.MapRoute(
                name: "Paging-myblog",
                template: "MyBlog/{id:int?}",
                defaults: new { controller = "Home", action = "MyBlog" });
                /*End*/

                /*Blog*/
                routes.MapRoute(
                name: "PagingPageOne-blog",
                template: "Blog",
                defaults: new { controller = "Blog", action = "Index", id = 1 }
                );

                routes.MapRoute(
                name: "Paging-blog",
                template: "Blog/{id:int?}",
                defaults: new { controller = "Blog", action = "Index" });
                /*End*/

                /*Blog Category*/
                routes.MapRoute(
                name: "PagingPageOne-blogcategory",
                template: "BlogCategory",
                defaults: new { controller = "BlogCategory", action = "Index", id = 1 }
                );

                routes.MapRoute(
                name: "Paging-blogcategory",
                template: "BlogCategory/{id:int?}",
                defaults: new { controller = "BlogCategory", action = "Index" });
                /*End*/

                /*View Blog*/
                routes.MapRoute(
                name: "View-Blog",
                template: "{url}-id-{id}",
                defaults: new { controller = "Home", action = "ViewBlog" });
                /*End*/

                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

                /*View Page*/
                routes.MapRoute(
                name: "View-Page",
                template: "{url}",
                defaults: new { controller = "Home", action = "Page" });
                /*End*/
            });
        }
    }
}
