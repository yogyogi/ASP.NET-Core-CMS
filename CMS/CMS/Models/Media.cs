using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace CMS.Models
{
    public partial class Media
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Url { get; set; }
        public string Title { get; set; }
        public string Alt { get; set; }
        public string Description { get; set; }

        public string ThumbUrl { get; set; }
        public List<SelectListItem> MediaDate { get; set; }
        public string Result { get; set; }
        public string DisplayUrl { get; set; }
        public string FileSize { get; set; }
        public string FileType { get; set; }
        public string Dimension { get; set; }


        public int? ParentId { get; set; }
        public DateTime AddedOn { get; set; }
    }
}
