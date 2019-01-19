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
using Microsoft.AspNetCore.Authorization;

namespace CMS.Controllers
{
    [Authorize(Roles = "User")]
    public class UserController : Controller
    {
        private UserManager<AppUser> userManager;
        private IUserValidator<AppUser> userValidator;
        private IPasswordValidator<AppUser> passwordValidator;
        private IPasswordHasher<AppUser> passwordHasher;
        private SignInManager<AppUser> signInManager;

        public UserController(UserManager<AppUser> userMgr, IUserValidator<AppUser> userValid, IPasswordValidator<AppUser> passValid, IPasswordHasher<AppUser> passwordHash, SignInManager<AppUser> signMgr)
        {
            userManager = userMgr;
            userValidator = userValid;
            passwordValidator = passValid;
            passwordHasher = passwordHash;
            signInManager = signMgr;
        }

        public IActionResult Index()
        {
            return View();
        }

        [AllowAnonymous]
        public ViewResult Create()
        {
            ViewBag.Title = "Create User";
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Create(User user)
        {
            if (ModelState.IsValid)
            {
                AppUser appUser = new AppUser
                {
                    UserName = user.UserName,
                    Email = user.Email,
                    PhoneNumber = user.PhoneNumber
                };

                IdentityResult result = await userManager.CreateAsync(appUser, user.Password);

                if (result.Succeeded)
                {
                    result = await userManager.AddToRoleAsync(appUser, "User");
                    if (result.Succeeded)
                    {
                        TempData["result"] = "Account Create - Now please login";
                        return RedirectToAction("Index", "Login");
                    }
                    else
                    {
                        foreach (IdentityError error in result.Errors)
                            ModelState.AddModelError("", error.Description);
                    }
                }
                else
                {
                    foreach (IdentityError error in result.Errors)
                        ModelState.AddModelError("", error.Description);
                }
            }
            return View(user);
        }

        public async Task<IActionResult> Edit()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);
            ViewBag.Title = "Update User";
            return View(user);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(AppUser appUser, string Password)
        {
            //AppUser user = await userManager.FindByIdAsync(id);
            ViewBag.Title = "Update User";
            AppUser user = await userManager.GetUserAsync(HttpContext.User);
            if (user != null)
            {
                user.Email = appUser.Email;
                IdentityResult validEmail = await userValidator.ValidateAsync(userManager, user);
                if (validEmail.Succeeded)
                    user.Email = appUser.Email;
                else
                    AddErrorsFromResult(validEmail);

                IdentityResult validPass = null;
                if (!string.IsNullOrEmpty(Password))
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, Password);
                    if (validPass.Succeeded)
                        user.PasswordHash = passwordHasher.HashPassword(user, Password);
                    else
                        AddErrorsFromResult(validPass);
                }
                else
                {
                    validPass = await passwordValidator.ValidateAsync(userManager, user, appUser.PasswordHash);
                    user.PasswordHash = appUser.PasswordHash;
                }

                if (validEmail.Succeeded && validPass.Succeeded)
                {
                    user.PhoneNumber = appUser.PhoneNumber;
                    user.Country = appUser.Country;
                    user.Age = appUser.Age;

                    IdentityResult result = await userManager.UpdateAsync(user);
                    if (result.Succeeded)
                        ViewBag.Result = "Update Successful";
                    else
                        AddErrorsFromResult(result);
                }
            }
            else
                ModelState.AddModelError("", "User Not Found");
            return View(user);
        }

        private void AddErrorsFromResult(IdentityResult result)
        {
            foreach (IdentityError error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        public async Task<IActionResult> Logout()
        {
            await signInManager.SignOutAsync();
            return RedirectToAction("Index", "Login");
        }
    }
}