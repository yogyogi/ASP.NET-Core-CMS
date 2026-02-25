using CMS.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

// DbContext
builder.Services.AddDbContext<CMSContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

/*Identity*/
builder.Services.AddDbContext<AppIdentityDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("IdentityConnection")));
builder.Services.AddIdentity<AppUser, IdentityRole>().AddEntityFrameworkStores<AppIdentityDbContext>().AddDefaultTokenProviders();
/*End*/

/*Identity Login Url */
builder.Services.ConfigureApplicationCookie(opts => opts.LoginPath = "/Login");

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.MapStaticAssets();

/*Identity*/
app.UseAuthentication();
app.UseAuthorization();
/*End*/

app.MapControllerRoute(
    name: "View-Blog",
    pattern: "b/{name}",
    defaults: new { controller = "Home", action = "ViewBlog" });

app.MapControllerRoute(
    name: "PagingPageOne",
    pattern: "{controller}",
    defaults: new { controller = "Home", action = "Index", id = 1 });

app.MapControllerRoute(
    name: "Paging",
    pattern: "{controller}/{id:int?}",
    defaults: new { controller = "Home", action = "Index" });

app.MapControllerRoute(
    name: "MatchUrl",
    pattern: "{name:required}",
    defaults: new { controller = "Home", action = "Page" });

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
