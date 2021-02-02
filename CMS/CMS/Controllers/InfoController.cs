using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CMS.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using System.Xml.Serialization;
using System.IO;
using Microsoft.AspNetCore.Mvc.Rendering;
using CMS.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;

namespace CMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class InfoController : Controller
    {
        private IWebHostEnvironment hostingEnvironment;

        public InfoController(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        public IActionResult Index()
        {
            ViewBag.MediaDate = GetMediaDate();
            Info info = new Info();

            if (!System.IO.File.Exists(System.IO.Path.Combine(hostingEnvironment.WebRootPath, "json/info.xml")))
                CreateXml(info);

            XmlSerializer xmlFormat = new XmlSerializer(typeof(Info));
            using (Stream stream = System.IO.File.OpenRead(System.IO.Path.Combine(hostingEnvironment.WebRootPath, "json/info.xml")))
            {
                info = (Info)xmlFormat.Deserialize(stream);
            }

            info.logo = info.logo != null ? info.logo : "/images/addphoto.jpg";
            return View(info);
        }

        [HttpPost]
        public IActionResult Index(Info info)
        {
            ViewBag.MediaDate = GetMediaDate();
            info.logo = info.logo == "/images/addphoto.jpg" ? null : info.logo;
            CreateXml(info);
            info.logo = info.logo != null ? info.logo : "/images/addphoto.jpg";
            ViewBag.Result = "Success";
            return View(info);
        }

        void CreateXml(Info info)
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(Info));
            using (Stream fStream = new FileStream(System.IO.Path.Combine(hostingEnvironment.WebRootPath, "json/info.xml"), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlFormat.Serialize(fStream, info);
            }
        }

        public List<SelectListItem> GetMediaDate()
        {
            List<SelectListItem> mediaDateList = new List<SelectListItem>();
            List<MediaDate> mdList = new List<MediaDate>();
            var context = new CMSContext();

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
    }
}