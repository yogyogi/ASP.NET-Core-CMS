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
using Microsoft.Data.SqlClient;
using CMS.Infrastructure;

namespace CMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class BlogController : Controller
    {
        public ActionResult Index(int? id, string searchText, int? status)
        {
            BlogList list = new BlogList();
            list = GetBlog(id, searchText, status);
            return View(list);
        }

        public ActionResult Add(int? id)
        {
            ViewBag.CategoryList = GetActiveCategory();
            ViewBag.MediaDate = GetMediaDate();

            Blog blog = new Blog();
            blog.PrimaryImageUrl = "/images/addphoto.jpg";
            if (id != null)
            {
                var context = new CMSContext();
                blog = context.Blog.Where(x => x.Id == id).FirstOrDefault();
                blog.PrimaryImageUrl = blog.PrimaryImageId != null ? "/" + context.Media.Where(x => x.Id == blog.PrimaryImageId).Select(x => x.Url).FirstOrDefault() : "/images/addphoto.jpg";
                ViewBag.Title = "Update Blog";
                return View(blog);
            }

            ViewBag.Title = "Add New Blog";
            return View(blog);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Add(Blog blog, int? id)
        {
            ViewBag.CategoryList = GetActiveCategory();
            ViewBag.MediaDate = GetMediaDate();

            if (ModelState.IsValid)
            {
                var context = new CMSContext();
                if (id == null)
                {
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = CommonFunction.Url(blog.Url)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Sep",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 1,
                                        Value = "-"
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@TableName",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 25,
                                        Value = DBNull.Value
                                    },
                                     new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = DBNull.Value
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@TempUrl",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 100
                                    }};
                    context.Database.ExecuteSqlRaw("[dbo].[sp_GetURL] @Url, @Sep, @TableName, @Id, @TempUrl out", param);

                    blog.Url = Convert.ToString(param[4].Value);
                    context.Blog.Add(blog);
                    int result = context.SaveChanges();

                    TempData["result"] = result == 1 ? "Insert Successful" : "Failed";
                    if (result == 1)
                        return RedirectToAction("Add", new { id = blog.Id });
                }
                else
                {
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = CommonFunction.Url(blog.Url)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Sep",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 1,
                                        Value = "-"
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@TableName",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 25,
                                        Value = "Blog"
                                    },
                                     new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = id
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@TempUrl",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 100
                                    }};
                    context.Database.ExecuteSqlRaw("[dbo].[sp_GetURL] @Url, @Sep, @TableName, @Id, @TempUrl out", param);

                    var blogResult = context.Blog.Where(x => x.Id == id).FirstOrDefault();
                    blogResult.Name = blog.Name;
                    blogResult.Url = Convert.ToString(param[4].Value);
                    blogResult.CategoryId = blog.CategoryId;
                    blogResult.PrimaryImageId = blog.PrimaryImageId;
                    blogResult.Description = blog.Description;
                    blogResult.MetaTitle = blog.MetaTitle;
                    blogResult.MetaKeyword = blog.MetaKeyword;
                    blogResult.MetaDescription = blog.MetaDescription;
                    blogResult.Status = blog.Status;

                    int result = context.SaveChanges();
                    ModelState.Clear();
                    TempData["result"] = result == 1 ? "Update Successful" : "Failed";
                }
                blog.PrimaryImageUrl = blog.PrimaryImageUrl ?? "~/images/addphoto.jpg";
            }
            ViewBag.Title = id == null ? "Add New Blog" : "Update Blog";
            return View(blog);
        }

        List<SelectListItem> GetActiveCategory()
        {
            List<SelectListItem> activeBlogCategory = new List<SelectListItem>();
            var context = new CMSContext();
            activeBlogCategory = context.BlogCategory.Where(x => x.Status == true).Select(x => new SelectListItem { Text = x.Name, Value = x.Id.ToString() }).ToList();
            return activeBlogCategory;
        }

        public List<SelectListItem> GetMediaDate()
        {
            List<SelectListItem> mediaDateList = new List<SelectListItem>();
            List<MediaDate> mdList = new List<MediaDate>();
            var context = new CMSContext();

            /*Does not work https://docs.microsoft.com/en-us/ef/core/querying/raw-sql
            DbSet<MediaDate> set = context.Set<MediaDate>();
            mediaDateList = set.FromSql("[dbo].[sp_GetMediaDate]").Select(x => new SelectListItem { Text = x.DateText, Value = x.DateValue }).ToList();
            */

            using (var cnn = context.Database.GetDbConnection())
            {
                var cmm = cnn.CreateCommand();
                cmm.CommandType = System.Data.CommandType.StoredProcedure;
                cmm.CommandText = "[dbo].[sp_GetMediaDate]";
                cmm.Connection = cnn;
                cnn.Open();
                var reader = cmm.ExecuteReader();

                while (reader.Read())
                {
                    MediaDate mediaDate = new MediaDate();
                    mediaDate.DateText = Convert.ToString(reader["DateText"]);
                    mediaDate.DateValue = Convert.ToString(reader["DateValue"]);
                    mdList.Add(mediaDate);
                }
            }

            mediaDateList = mdList.Select(x => new SelectListItem { Text = x.DateText, Value = x.DateValue }).ToList();
            return mediaDateList;
        }

        public string GetMediaWithPaging(string date, int page, string name)
        {
            //http://webdeveloperplus.com/jquery/create-a-dynamic-scrolling-content-box-using-ajax/ http://stackoverflow.com/questions/8480466/how-to-check-if-scrollbar-is-at-the-bottom
            //http://stackoverflow.com/questions/19933115/mvc-4-postback-on-dropdownlist-change
            System.Text.StringBuilder stringBuilder = new System.Text.StringBuilder();
            var context = new CMSContext();
            var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@PageNo",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = page
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@PageSize",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = "35"
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Name",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = name ?? (object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@FileType",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 10,
                                        Value = "image"
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@MediaDateSearch",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 10,
                                        Value =  date == "null" ? "all" : date
                                    }};
            var mediaList = context.Media.FromSqlRaw("[dbo].[sp_GetMediaWithPaging] @PageNo, @PageSize, @Name, @FileType, @MediaDateSearch", param);
            foreach (Media media in mediaList)
            {
                string url = media.ThumbUrl == null ? media.Url : media.ThumbUrl;
                stringBuilder.Append("<li class=\"item col-sm-3\"><input type=\"checkbox\"/><a target=\"_blank\" href=\"" + Url.Action("Update", "Media", new { id = media.Id }) + "\"><img data-url=\"/" + media.Url + "\" width =\"135\" src=\"" + Url.Content("~/" + url) + "\"/></a></li>");
            }

            return stringBuilder.ToString();
        }

        public BlogList GetBlog(int? page, string searchText, int? status)
        {
            int pageSize = 3;
            int pageNo = page == null ? 1 : Convert.ToInt32(page);

            //int total;
            //using (var context = new CMSEntities())
            //{
            //    total = context.Blogs.Where(x => x.Name == (searchText == null ? x.Name : searchText) && x.Status == (status == null ? x.Status : (status == 1 ? true : false))).Count();
            //}

            var skip = pageSize * (Convert.ToInt32(pageNo) - 1);
            //var canPage = skip < total;

            BlogList bList = new BlogList();
            //if (canPage)
            //{
            using (var context = new CMSContext())
            {
               // bool searchVal = string.IsNullOrEmpty(searchText);

                var result = context.Blog.Where(x => x.Status == (status == null ? x.Status : (status == 1 ? true : false)) && x.Name.Contains(searchText == null ? x.Name : searchText)).OrderByDescending(x => x.Id).Skip(skip).Take(pageSize).ToList();

                int total = context.Blog.Where(x => x.Status == (status == null ? x.Status : (status == 1 ? true : false)) && x.Name.Contains(searchText == null ? x.Name : searchText)).Count();

                PagingInfo pagingInfo = new PagingInfo();
                pagingInfo.CurrentPage = pageNo;
                pagingInfo.TotalItems = total;
                pagingInfo.ItemsPerPage = pageSize;

                bList.blog = result;
                bList.allTotal = context.Blog.Count();
                bList.activeTotal = context.Blog.Where(x => x.Status == true).Count();
                bList.inactiveTotal = context.Blog.Where(x => x.Status == false).Count();
                bList.searchText = searchText;
                bList.status = status;
                bList.pagingInfo = pagingInfo;
            }
            //}
            return bList;
        }

        public string UpdateBulkStatus(string idChecked, string statusToChange)
        {
            string result = "";
            if (statusToChange == "Status")
                result = "Select Status";
            else if (idChecked == "")
                result = "Select at least 1 item";
            else
            {
                using (var context = new CMSContext())
                {
                    var blog = context.Blog.Where(x => idChecked.Contains(x.Id.ToString())).ToList();
                    blog.ForEach(x => x.Status = Convert.ToBoolean(Convert.ToInt32(statusToChange)));
                    context.SaveChanges().ToString();
                    result = "Success";
                }
            }
            return result;
        }
    }
}