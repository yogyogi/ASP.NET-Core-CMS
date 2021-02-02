using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using CMS.Models;
using CMS.Models.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using CMS.Infrastructure;
using Microsoft.AspNetCore.Identity;

namespace CMS.Controllers
{
    public class HomeController : Controller
    {
        private UserManager<AppUser> userManager;
        public HomeController(UserManager<AppUser> userMgr)
        {
            userManager = userMgr;
        }

        public IActionResult Index()
        {
            Page page = new Page();
            using (var context = new CMSContext())
            {
                page = context.Page.Where(t => t.Name == "Home").FirstOrDefault();
            }
            return View(page);
        }

        public IActionResult Page(string name)
        {
            Page page = new Page();
            using (var context = new CMSContext())
            {
                page = context.Page.Where(t => t.Url == name).FirstOrDefault();
            }
            return View("Index", page);
        }

        public IActionResult ViewBlog(string name)
        {
            Blog blog = new Blog();
            using (var context = new CMSContext())
            {
                blog = context.Blog.Where(t => t.Url == name).FirstOrDefault();
                blog.PrimaryImageUrl = blog.PrimaryImageId != null ? "/" + context.Media.Where(x => x.Id == blog.PrimaryImageId).Select(x => x.Url).FirstOrDefault() : "/images/addphoto.jpg";

                ViewBag.BlogCategory = context.BlogCategory.Where(t => t.Status == true).ToList();
            }
            return View(blog);
        }

        public IActionResult MyBlogCategory(int id, string url)
        {
            BlogList list = new BlogList();

            BlogCategory blogCategory = new BlogCategory();
            using (var context = new CMSContext())
            {
                blogCategory = context.BlogCategory.Where(x => x.Url == url).FirstOrDefault();
            }

            list = GetBlog(id, null, 1, blogCategory.Id);

            ViewData["Meta"]= new string[3] { blogCategory.Name, "", "Welcome to My Blogs" };
            ViewBag.url = url;
            return View("MyBlog", list);
        }

        public IActionResult MyBlog(int id)
        {
            BlogList list = new BlogList();
            list = GetBlog(id, null, 1, 0);

            ViewData["Meta"] = new string[3] { "My blogs", "", "Welcome to My Blogs" };
            return View(list);
        }

        public BlogList GetBlog(int? page, string searchText, int? status, int blogCategoryId)
        {

            int pageSize = 3;
            int pageNo = page == null ? 1 : Convert.ToInt32(page);

            var skip = pageSize * (Convert.ToInt32(pageNo) - 1);

            BlogList bList = new BlogList();
            using (var context = new CMSContext())
            {
                var result = context.Blog.Where(x => x.Status == (status == null ? x.Status : (status == 1 ? true : false)) && x.Name.Contains(searchText == null ? x.Name : searchText) && (blogCategoryId == 0 || x.CategoryId == blogCategoryId)).OrderByDescending(x => x.Id).Skip(skip).Take(pageSize).ToList();
                result.ForEach(u => u.PrimaryImageUrl = u.PrimaryImageId != null ? "/" + context.Media.Where(x => x.Id == u.PrimaryImageId).Select(x => x.Url).FirstOrDefault() : "/images/addphoto.jpg");

                int total = context.Blog.Where(x => x.Status == (status == null ? x.Status : (status == 1 ? true : false)) && x.Name.Contains(searchText == null ? x.Name : searchText) && (blogCategoryId == 0 || x.CategoryId == blogCategoryId)).Count();

                PagingInfo pagingInfo = new PagingInfo();
                pagingInfo.CurrentPage = pageNo;
                pagingInfo.TotalItems = total;
                pagingInfo.ItemsPerPage = pageSize;

                bList.blog = result;
                bList.allTotal = context.Blog.Count();
                bList.activeTotal = context.Blog.Where(x => x.Status == true).Count();
                bList.inactiveTotal = context.Blog.Where(x => x.Status == false).Count();
                bList.searchText = searchText;
                bList.status = null;
                bList.pagingInfo = pagingInfo;
            }
            return bList;
        }
    }
}