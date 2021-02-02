using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;

namespace CMS.Infrastructure
{
    public class CommonFunction
    {
        private IWebHostEnvironment hostingEnvironment;
        public CommonFunction(IWebHostEnvironment env)
        {
            hostingEnvironment = env;
        }

        public static string Url(string value)
        {
            string output = "";
            output = Regex.Replace(FormatUrl(value).Replace(" ", "-"), "[-]{2,}", "-").ToLower();
            return output;
        }
        public static string FormatUrl(string url)
        {
            return Regex.Replace(url, @"([?<>*.%,;:/&'()""]+)+", "");
        }
        public static string RemoveMainDirectoryName(string value)
        {
            return value.Replace("cms/", "");
        }
        public void CreateUploadDirectory()
        {
            string currectYear = Convert.ToString(DateTime.Now.Year);
            string currectMonth = Convert.ToString(DateTime.Now.Month);
            string yearPath = "Upload/" + currectYear + "";
            string monthPath = "Upload/" + currectYear + "/" + currectMonth + "";

            bool yearFolderExist = System.IO.Directory.Exists(System.IO.Path.Combine(hostingEnvironment.WebRootPath, yearPath));
            if (!yearFolderExist)
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(hostingEnvironment.WebRootPath, yearPath));

            bool monthFolderExist = System.IO.Directory.Exists(System.IO.Path.Combine(hostingEnvironment.WebRootPath, monthPath));
            if (!monthFolderExist)
                System.IO.Directory.CreateDirectory(System.IO.Path.Combine(hostingEnvironment.WebRootPath, monthPath));
        }
    }
}
