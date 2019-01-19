using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public partial class BlogCategory
    {
        public int Id { get; set; }
        public int? ParentId { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Url { get; set; }
        public string MetaTitle { get; set; }
        public string MetaKeyword { get; set; }
        public string MetaDescription { get; set; }

        [Required]
        public string Description { get; set; }
        public DateTime AddedOn { get; set; }

        [Required]
        public bool Status { get; set; }
    }
}
