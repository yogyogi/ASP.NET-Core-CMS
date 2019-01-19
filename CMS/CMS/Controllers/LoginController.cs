using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using CMS.Models;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using CMS.Models.ViewModels;

namespace CMS.Controllers
{
    public class LoginController : Controller
    {
        private UserManager<AppUser> userManager;
        private SignInManager<AppUser> signInManager;
        private RoleManager<IdentityRole> roleManager;

        public LoginController(UserManager<AppUser> userMgr, SignInManager<AppUser> signinMgr, RoleManager<IdentityRole> roleMgr)
        {
            userManager = userMgr;
            signInManager = signinMgr;
            roleManager = roleMgr;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Login login)
        {
            if (ModelState.IsValid)
            {
                AppUser user = await userManager.FindByNameAsync(login.Name);
                if (user != null)
                {
                    await signInManager.SignOutAsync();
                    Microsoft.AspNetCore.Identity.SignInResult result = await signInManager.PasswordSignInAsync(user, login.Password, false, false);

                    if (result.Succeeded)
                    {
                        var roles = await userManager.GetRolesAsync(user);
                        if (roles.FirstOrDefault() == "Admin")
                            return Redirect(login.ReturnUrl == "/" ? "/Admin" : login.ReturnUrl);
                        else
                            return Redirect(login.ReturnUrl == "/" ? "/User" : login.ReturnUrl);
                    }
                }
            }
            ModelState.AddModelError("", "Invalid name or password");
            return View(login);
        }

        public async Task<IActionResult> Create()
        {
            IdentityResult result = await roleManager.CreateAsync(new IdentityRole("Admin"));

            AppUser user = new AppUser
            {
                UserName = "admin",
                Email = "admin@gmail.com"
            };
            result = await userManager.CreateAsync(user, "Secret123$");

            if (result.Succeeded)
            {
                result = await userManager.AddToRoleAsync(user, "Admin");
                ViewBag.Result = "User Created";
            }
            else
            {
                foreach (IdentityError error in result.Errors)
                    ModelState.AddModelError("", error.Description);
            }
            return View();
        }
        public async Task<IActionResult> Delete()
        {
            AppUser user = await userManager.FindByNameAsync("Admin");
            IdentityResult result = await userManager.DeleteAsync(user);
            return View();
        }
    }
}