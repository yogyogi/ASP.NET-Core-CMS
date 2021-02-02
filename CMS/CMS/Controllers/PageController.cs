using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CMS.Models;
using Microsoft.Data.SqlClient;
using CMS.Models.ViewModels;
using CMS.Infrastructure;
using Microsoft.EntityFrameworkCore;

namespace CMS.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PageController : Controller
    {
        public IActionResult Index(int? id, string searchText, int? status)
        {
            PageList pageList = new PageList();
            pageList = GetPage(id, searchText, status);
            return View(pageList);
        }

        public IActionResult Add(int? id)
        {
            Page page = new Page();
            if (id != null)
            {
                using (var context = new CMSContext())
                {
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = id
                                    }};
                    page = context.Page.FromSqlRaw("[dbo].[sp_GetPageById] @Id", param).AsEnumerable().FirstOrDefault();
                    ViewBag.Title = "Update Page";
                    return View(page);
                }
            }
            ViewBag.Title = "Add New Page";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Add(Page page, int? id)
        {
            if (ModelState.IsValid)
            {
                using (var context = new CMSContext())
                {
                    if (id == null)
                    {
                        var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Name",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=100,
                                        Value = page.Name
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=100,
                                        Value = CommonFunction.Url(page.Url)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@MetaTitle",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=250,
                                        Value = page.MetaTitle ?? (object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@MetaKeyword",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=250,
                                        Value = page.MetaKeyword ?? (object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@MetaDescription",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=250,
                                        Value = page.MetaDescription ?? (object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Description",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=-1,
                                        Value = page.Description
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Status",
                                        SqlDbType =  System.Data.SqlDbType.Bit,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = page.Status
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Result",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size=50
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@CreatedId",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Output,
                                    }};

                        context.Database.ExecuteSqlRaw("[dbo].[sp_InsertPage] @Name, @Url, @MetaTitle, @MetaKeyword, @MetaDescription, @Description, @Status, @Result out, @CreatedId out", param);

                        TempData["result"] = Convert.ToString(param[7].Value);
                        if (Convert.ToString(param[7].Value) == "Insert Successful")
                            return RedirectToAction("Add", new { id = Convert.ToInt32(param[8].Value) });
                    }
                    else
                    {
                        var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = page.Id
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Name",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=100,
                                        Value = page.Name
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Url",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=100,
                                        Value =  CommonFunction.Url(page.Url)
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@MetaTitle",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=250,
                                        Value = page.MetaTitle ?? (object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@MetaKeyword",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=250,
                                        Value = page.MetaKeyword ?? (object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@MetaDescription",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=250,
                                        Value = page.MetaDescription ?? (object)DBNull.Value
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Description",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=-1,
                                        Value = page.Description
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Status",
                                        SqlDbType =  System.Data.SqlDbType.Bit,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = page.Status
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Result",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size=50
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@CreatedUrl",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size=100
                                    }};
                        context.Database.ExecuteSqlRaw("[dbo].[sp_UpdatePage] @Id, @Name, @Url, @MetaTitle, @MetaKeyword, @MetaDescription, @Description, @Status, @Result out, @CreatedUrl out", param);
                        ModelState.Clear();
                        page.Url = Convert.ToString(param[9].Value);
                        TempData["result"] = Convert.ToString(param[8].Value);
                    }
                }
            }
            ViewBag.Title = id == null ? "Add New Page" : "Update Page";
            return View(page);
        }

        public string UpdateBulkPageStatus(string idChecked, string statusToChange)
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
                    var param = new SqlParameter[] {
                                    new SqlParameter() {
                                        ParameterName = "@Id",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size = 50,
                                        Value = idChecked
                                    },
                                    new SqlParameter() {
                                        ParameterName = "@Status",
                                        SqlDbType =  System.Data.SqlDbType.Bit,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value =Convert.ToBoolean(Convert.ToInt32(statusToChange))
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@Result",
                                        SqlDbType = System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Output,
                                        Size = 50
                                    }
                    };
                    context.Database.ExecuteSqlRaw("[dbo].[sp_UpdateBulkPageStatus] @Id, @Status, @Result out", param);
                    result = Convert.ToString(param[2].Value);
                }
            }
            return result;
        }

        public PageList GetPage(int? page, string searchText, int? status)
        {
            int pageSize = 3;
            int pageNo = page == null ? 1 : Convert.ToInt32(page);

            List<Page> list = new List<Page>();
            PageExtraData pageExtraData = new PageExtraData();
            PageList pageList = new PageList();
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
                                        ParameterName = "@Name",
                                        SqlDbType =  System.Data.SqlDbType.VarChar,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Size=100,
                                        Value = searchText ?? (object)DBNull.Value
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@PageSize",
                                        SqlDbType = System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                        Value = pageSize
                                    },
                                    new SqlParameter()
                                    {
                                        ParameterName = "@Status",
                                        SqlDbType = System.Data.SqlDbType.Int,
                                        Direction = System.Data.ParameterDirection.Input,
                                         Value=status ?? (object)DBNull.Value
                                    }
                };
                using (var cnn = context.Database.GetDbConnection())
                {
                    var cmm = cnn.CreateCommand();
                    cmm.CommandType = System.Data.CommandType.StoredProcedure;
                    cmm.CommandText = "[dbo].[sp_GetPageWithPaging]";
                    cmm.Parameters.AddRange(param);
                    cmm.Connection = cnn;
                    cnn.Open();
                    var reader = cmm.ExecuteReader();

                    while (reader.Read())
                    {
                        Page pages = new Page();
                        pages.Id = Convert.ToInt32(reader["Id"]);
                        pages.Name = Convert.ToString(reader["Name"]);
                        pages.Url = Convert.ToString(reader["Url"]);
                        pages.MetaTitle = Convert.ToString(reader["MetaTitle"]);
                        pages.MetaKeyword = Convert.ToString(reader["MetaKeyword"]);
                        pages.MetaDescription = Convert.ToString(reader["MetaDescription"]);
                        pages.Description = Convert.ToString(reader["Description"]);
                        pages.AddedOn = Convert.ToDateTime(reader["AddedOn"]);
                        pages.Status = Convert.ToBoolean(reader["Status"]);
                        list.Add(pages);
                    }
                    reader.NextResult();
                    while (reader.Read())
                    {
                        PagingInfo pagingInfo = new PagingInfo();
                        pagingInfo.CurrentPage = pageNo;
                        pagingInfo.TotalItems = Convert.ToInt32(reader["Total"]);
                        pagingInfo.ItemsPerPage = pageSize;

                        pageList.page = list;
                        pageList.allTotalPage = Convert.ToInt32(reader["AllTotalPage"]);
                        pageList.activeTotalPage = Convert.ToInt32(reader["ActiveTotalPage"]);
                        pageList.inactiveTotalPage = Convert.ToInt32(reader["InActiveTotalPage"]);
                        pageList.searchText = searchText;
                        pageList.status = status;
                        pageList.pagingInfo = pagingInfo;
                    }
                }
                //var result = context.sp_GetPageWithPaging(pageNo, searchText, pageSize, status);
                //list = result.Select(x => new Models.Page() { id = x.Id, name = x.Name, url = x.Url, metaTitle = x.MetaTitle, metaKeyword = x.MetaKeyword, metaDescription = x.MetaDescription, description = x.Description, addedOn = x.AddedOn, status = Convert.ToInt32(x.Status) }).ToList();

                //var pageExtraDataResult = result.GetNextResult<sp_PageExtraData>();
                //pageExtraData = pageExtraDataResult.ToList().FirstOrDefault();
            }
            return pageList;
        }
    }
}