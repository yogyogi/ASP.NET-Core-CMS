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
using System.Security.Claims;

namespace CMS.Components
{
    public class UserDetail: ViewComponent
    {
        private UserManager<AppUser> userManager;
        public UserDetail(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            //string userName = HttpContext.User.Identity.Name;
            return View(await GetUser());
        }

        public async Task<LoggedUser> GetUser()
        {
            AppUser user = await userManager.GetUserAsync(HttpContext.User);
            var roles = await userManager.GetRolesAsync(user);

            LoggedUser loggedUser = new LoggedUser();
            loggedUser.UserName = user.UserName;
            loggedUser.Role = roles.FirstOrDefault();
           
            return loggedUser;
        }
    }
}
