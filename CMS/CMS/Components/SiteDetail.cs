using CMS.Infrastructure;
using CMS.Models;
using CMS.Models.ViewModels;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace CMS.Components
{
    public class SiteInfo : ViewComponent
    {
        private IWebHostEnvironment hostingEnvironment;

        public SiteInfo(IWebHostEnvironment environment)
        {
            hostingEnvironment = environment;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
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

        void CreateXml(Info info)
        {
            XmlSerializer xmlFormat = new XmlSerializer(typeof(Info));
            using (Stream fStream = new FileStream(System.IO.Path.Combine(hostingEnvironment.WebRootPath, "json/info.xml"), FileMode.Create, FileAccess.Write, FileShare.None))
            {
                xmlFormat.Serialize(fStream, info);
            }
        }
    }

    public class SiteMenu : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            Menu menu = new Menu();
            using (var context = new CMSContext())
            {
                menu =  await context.Menu.Where(x => x.Name == "Main" && x.Status == true).FirstOrDefaultAsync();
            }
            return View((object)BindMenu(menu));
        }

        string BindMenu(Menu menu)
        {
            //json has [] therore changed the below code accordingly
            if (menu == null)
                return "";
            var rootObject = JsonConvert.DeserializeObject<List<MenuJsonRoot>>(menu.Item);
            string mainString = "<ul>";

            for (int i = 0; i < rootObject.Count; i++)
            {
                var children = rootObject[i].children;
                if (children != null)
                {
                    string childString = "";
                    for (int j = 0; j < children.Count; j++)
                        childString = childString + CreateMenuItem(children[j]);
                    childString = "<ul>" + childString + "</ul>";

                    string parentString = "";
                    MenuJsonChild child = new MenuJsonChild();
                    child.deleted = rootObject[i].deleted;
                    child.@new = rootObject[i].@new;
                    child.slug = rootObject[i].slug;
                    child.name = rootObject[i].name;
                    child.id = rootObject[i].id;
                    parentString = CreateMenuItem(child).Replace("</li>", "") + childString + "</li>";
                    mainString = mainString + parentString;
                }
                else
                {
                    string parentString = "";
                    MenuJsonChild child = new MenuJsonChild();
                    child.deleted = rootObject[i].deleted;
                    child.@new = rootObject[i].@new;
                    child.slug = rootObject[i].slug;
                    child.name = rootObject[i].name;
                    child.id = rootObject[i].id;

                    parentString = CreateMenuItem(child);
                    mainString = mainString + parentString;
                }
            }
            mainString = mainString + "</ul>";
            return mainString;
        }

        string CreateMenuItem(MenuJsonChild child)
        {
            string childString = "<li><a href=\"/" + child.slug + "\">" + child.name + "</a></li>";
            return childString;
        }
    }
}
