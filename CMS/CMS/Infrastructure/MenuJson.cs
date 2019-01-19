using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Infrastructure
{
    public class MenuJson
    {
    }

    public class MenuJsonChild
    {
        public int deleted { get; set; }
        public int @new { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string id { get; set; }
    }

    public class MenuJsonRoot
    {
        public int deleted { get; set; }
        public int @new { get; set; }
        public string slug { get; set; }
        public string name { get; set; }
        public string id { get; set; }
        public List<MenuJsonChild> children { get; set; }
    }
}
