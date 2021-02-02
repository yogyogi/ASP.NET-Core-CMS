using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CMS.Models;
using CMS.Models.ViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Web;
using Microsoft.AspNetCore.Http;
using CMS.Infrastructure;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using System.Text;
using System.Drawing;
using System.Drawing.Drawing2D;

namespace CMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class MediaController : Controller
    {
        private IWebHostEnvironment hostingEnvironment;

        public MediaController(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        public ActionResult Index()
        {
            Media media = new Media();
            media.MediaDate = GetMediaDate();
            media.Result = GetMediaWithPaging("all", "all", 1, null);
            return View(media);
        }

        [HttpPost]
        public ActionResult Index(Models.Media mMedia, string date)
        {
            mMedia.MediaDate = GetMediaDate();
            return View(mMedia);
        }

        public ActionResult Add()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(IFormFile[] mediaUpload)
        {
            List<FileUpload> lFileUpload = new List<FileUpload>();

            if (mediaUpload[0] == null)
                ViewBag.Result = "No Files Selected";
            else
            {
                foreach (IFormFile file in mediaUpload)
                {
                    FileUpload fileUpload = new FileUpload();
                    /*Start inserting of the files*/
                    string[] resultInsert = InsertMedia(file, null, null);
                    string resultUpload = "";
                    if (resultInsert[0] == "Success")
                        resultUpload = await UploadMedia(file, resultInsert[1]);
                    /*End*/

                    fileUpload.name = file.FileName;
                    fileUpload.id = Convert.ToInt32(resultInsert[2]);

                    string savedPath150x150 = "";
                    bool isImage = IsImage(file.FileName);
                    if (isImage)
                    {
                        savedPath150x150 = "~/" + GetPath() + resultInsert[1];// remove this line when doing cropping

                        //Start the Cropping of Image
                        Dictionary<string, bool> dictionary = CheckFileCropApplicable(file);
                        string originalFileName = resultInsert[1];
                        string newFileName150x150 = originalFileName.Substring(0, originalFileName.LastIndexOf('.')) + "150X150" + originalFileName.Substring(originalFileName.LastIndexOf('.'), originalFileName.Length - originalFileName.LastIndexOf('.'));
                        string newFileName250x250 = originalFileName.Substring(0, originalFileName.LastIndexOf('.')) + "250X250" + originalFileName.Substring(originalFileName.LastIndexOf('.'), originalFileName.Length - originalFileName.LastIndexOf('.'));
                        string newFileName500x500 = originalFileName.Substring(0, originalFileName.LastIndexOf('.')) + "500X500" + originalFileName.Substring(originalFileName.LastIndexOf('.'), originalFileName.Length - originalFileName.LastIndexOf('.'));

                        foreach (var item in dictionary)
                        {
                            if ((item.Key == "Crop150") && (item.Value == true))
                            {
                                resultInsert = InsertMedia(file, newFileName150x150, fileUpload.id);
                                savedPath150x150 = CropImage(GetPath(), originalFileName, resultInsert[1], 150, 150, true);
                            }
                            else if ((item.Key == "Crop250") && (item.Value == true))
                            {
                                resultInsert = InsertMedia(file, newFileName250x250, fileUpload.id);
                                CropImage(GetPath(), originalFileName, resultInsert[1], 250, 250, true);
                            }
                            else if ((item.Key == "Crop500") && (item.Value == true))
                            {
                                resultInsert = InsertMedia(file, newFileName500x500, fileUpload.id);
                                CropImage(GetPath(), originalFileName, resultInsert[1], 500, 500, true);
                            }
                        }
                        //End
                    }
                    fileUpload.url = savedPath150x150 == "" ? "~/images/file-icon.png" : savedPath150x150;
                    lFileUpload.Add(fileUpload);
                }
            }
            if (lFileUpload.Count > 0)
                ViewBag.Result = lFileUpload.Count + " Files Uploaded";
            return View(lFileUpload);
        }

        public ActionResult Update(int id)
        {
            Media media = new Media();
            media = GetMediaById(id);
            ViewBag.Title = "Update Page";
            return View(media);
        }

        [HttpPost]
        public ActionResult Update(Media media)
        {
            if (IsImage(media.Name))
                media.DisplayUrl = media.Url;
            else
                media.DisplayUrl = "images/file-icon.png";

            if (ModelState.IsValid)
            {
                var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = media.Id
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Name",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = media.Name
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 250,
                                        Value = media.Url
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Title",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = media.Title??(object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Alt",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = media.Alt??(object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Description",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = media.Description??(object)DBNull.Value
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@Result",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 50
                                    }};
                using (var context = new CMSContext())
                {
                    context.Database.ExecuteSqlRaw("[dbo].[sp_UpdateMedia] @Id, @Name, @Url, @Title, @Alt, @Description, @Result out", param);
                }
                ViewBag.Title = "Update Media";
                ViewBag.result = Convert.ToString(param[6].Value);
            }
            return View(media);
        }

        public string DeleteMedia(string ids, string paths)
        {
            using (var context = new CMSContext())
            {
                var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Size=50,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = ids
                                    }};
                context.Database.ExecuteSqlRaw("[dbo].[sp_DeleteMedia] @Id", param);
            }
            foreach (string path in paths.Split(','))
                System.IO.File.Delete(Path.Combine(hostingEnvironment.WebRootPath, path.Substring(1)));
            return "Success";
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

        public string GetMediaWithPaging(string fileType, string date, int page, string name)
        {
            //http://webdeveloperplus.com/jquery/create-a-dynamic-scrolling-content-box-using-ajax/ http://stackoverflow.com/questions/8480466/how-to-check-if-scrollbar-is-at-the-bottom
            //http://stackoverflow.com/questions/19933115/mvc-4-postback-on-dropdownlist-change
            StringBuilder stringBuilder = new StringBuilder();
            using (var context = new CMSContext())
            {
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
                                        Value = 35
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
                                        Value = fileType
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@MediaDateSearch",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 10,
                                        Value = date=="null" ? "all" : date
                                    }};
                var result = context.Media.FromSqlRaw("[dbo].[sp_GetMediaWithPaging] @PageNo, @PageSize, @Name, @FileType, @MediaDateSearch", param);

                foreach (Media item in result)
                {
                    bool isImage = IsImage(item.Name);
                    string url = "";
                    if (isImage)
                        url = item.ThumbUrl == null ? item.Url : item.ThumbUrl;
                    else
                        url = "images/file-icon.png";
                    stringBuilder.Append("<li class=\"item\"><a href=\"" + Url.Action("Update", new { id = item.Id }) + "\"><img data-url=\"/" + item.Url + "\" width =\"135\" src=\"" + Url.Content("~/" + url) + "\"/></a></li>");
                }
            }

            return stringBuilder.ToString();

        }

        public static bool IsImage(string fileName)
        {
            bool result = new bool();
            string targetExtension = Path.GetExtension(fileName);
            List<string> ImageExtensions = new List<string> { ".JPG", ".JPEG", ".BMP", ".GIF", ".PNG" };
            if (ImageExtensions.Contains(targetExtension.ToUpperInvariant()))
                result = true;
            return result;
        }

        public string[] InsertMedia(IFormFile file, string fileName, int? parentId)
        {
            using (var context = new CMSContext())
            {
                var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Name",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=100,
                                        Value = fileName == null ? file.FileName : fileName
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=250,
                                        Value = GetPath() + (fileName == null ? file.FileName : fileName)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Title",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Alt",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Description",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 100,
                                        Value = DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@ParentId",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = parentId ?? (object)DBNull.Value
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@Result",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 50
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@CreatedFileName",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 100
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@CreatedId",
                                        SqlDbType = System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Output
                                    }};
                context.Database.ExecuteSqlRaw("[dbo].[sp_InsertMedia] @Name, @Url, @Title, @Alt, @Description, @ParentId, @Result out, @CreatedFileName out, @CreatedId out", param);
                return new string[] { Convert.ToString(param[6].Value), Convert.ToString(param[7].Value), Convert.ToString(param[8].Value) };
            }
        }

        public async Task<string> UploadMedia(IFormFile file, string fileName)
        {
            CommonFunction commonFunction = new CommonFunction(hostingEnvironment);
            commonFunction.CreateUploadDirectory();
            string path = Path.Combine(hostingEnvironment.WebRootPath, GetPath() + fileName);

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return "Success";
        }

        public string GetPath()
        {
            string currectYear = Convert.ToString(DateTime.Now.Year);
            string currectMonth = Convert.ToString(DateTime.Now.Month);
            string path = "Upload/" + currectYear + "/" + currectMonth + "/";
            return path;
        }

        public string CropImage(string originalPath, string originalFileName, string newFileName, int Width, int Height, bool needToFill)
        {
            using (var image = Image.FromFile(Path.Combine(hostingEnvironment.WebRootPath, originalPath + originalFileName)))
            {
                int sourceWidth = image.Width;
                int sourceHeight = image.Height;
                int sourceX = 0;
                int sourceY = 0;
                double destX = 0;
                double destY = 0;

                double nScale = 0;
                double nScaleW = 0;
                double nScaleH = 0;

                nScaleW = ((double)Width / (double)sourceWidth);
                nScaleH = ((double)Height / (double)sourceHeight);
                if (!needToFill)
                    nScale = Math.Min(nScaleH, nScaleW);
                else
                {
                    nScale = Math.Max(nScaleH, nScaleW);
                    destY = (Height - sourceHeight * nScale) / 2;
                    destX = (Width - sourceWidth * nScale) / 2;
                }

                int destWidth = (int)Math.Round(sourceWidth * nScale);
                int destHeight = (int)Math.Round(sourceHeight * nScale);

                Bitmap bmPhoto = null;
                bmPhoto = new Bitmap(destWidth + (int)Math.Round(2 * destX), destHeight + (int)Math.Round(2 * destY));

                using (Graphics grPhoto = Graphics.FromImage(bmPhoto))
                {
                    grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    grPhoto.CompositingQuality = CompositingQuality.HighQuality;
                    grPhoto.SmoothingMode = SmoothingMode.HighQuality;

                    Rectangle to = new Rectangle((int)Math.Round(destX), (int)Math.Round(destY), destWidth, destHeight);
                    Rectangle from = new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight);
                    grPhoto.DrawImage(image, to, from, GraphicsUnit.Pixel);
                    image.Dispose();
                }

                //Crops the image
                string savedPath = originalPath + newFileName;
                bmPhoto.Save(Path.Combine(hostingEnvironment.WebRootPath, savedPath));
                return "~/" + savedPath;
            }
        }

        public Dictionary<string, bool> CheckFileCropApplicable(IFormFile file)
        {
            //https://github.com/CoreCompat/CoreCompat 
            //https://www.nuget.org/packages/System.Drawing.Common
            Dictionary<string, bool> dictionary = new Dictionary<string, bool>();
            Image image = Image.FromStream(file.OpenReadStream());
            if ((image.Width < 150) && (image.Height < 150))
                dictionary.Add("Crop150", false);
            else
                dictionary.Add("Crop150", true);
            if ((image.Width < 250) && (image.Height < 250))
                dictionary.Add("Crop250", false);
            else
                dictionary.Add("Crop250", true);
            if ((image.Width < 500) && (image.Height < 500))
                dictionary.Add("Crop500", false);
            else
                dictionary.Add("Crop500", true);
            return dictionary;
        }

        public Media GetMediaById(int id)
        {
            Media media = new Media();
            using (var context = new CMSContext())
            {
                var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = id
                                    }};
                var resultList = context.Media.FromSqlRaw("[dbo].[sp_GetMediaById] @id", param).AsEnumerable();

                if (resultList != null)
                {
                    media = resultList.AsEnumerable().FirstOrDefault();
                    media.DisplayUrl = "images/file-icon.png";

                    long fileSize = new FileInfo(Path.Combine(hostingEnvironment.WebRootPath, Convert.ToString(resultList.Select(x => x.Url).FirstOrDefault()))).Length;

                    float fileSizeValue;
                    if (fileSize > 1024 * 1024)
                    {
                        fileSizeValue = (float)fileSize / (1024 * 1024);
                        media.FileSize = fileSizeValue.ToString("0.00") + "MB";
                    }
                    else if (fileSize > 1024)
                    {
                        fileSizeValue = (float)fileSize / 1024;
                        media.FileSize = fileSizeValue.ToString("0.00") + "KB";
                    }
                    else
                    {
                        fileSizeValue = fileSize;
                        media.FileSize = fileSizeValue + "Bytes";
                    }

                    media.FileType = Path.GetExtension(Convert.ToString(resultList.Select(x => x.Name).FirstOrDefault()));
                    if (IsImage(Convert.ToString(resultList.Select(x => x.Name).FirstOrDefault())))
                    {
                        var image = Image.FromFile(Path.Combine(hostingEnvironment.WebRootPath, Convert.ToString(resultList.Select(x => x.Url).FirstOrDefault())));
                        media.Dimension = image.Width + "*" + image.Height;
                        media.DisplayUrl = Convert.ToString(resultList.Select(x => x.Url).FirstOrDefault());
                    }
                }
            }
            return media;
        }
    }
}