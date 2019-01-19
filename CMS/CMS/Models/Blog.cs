using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public partial class Blog
    {
        public int Id { get; set; }

        [Required]
        public int CategoryId { get; set; }
        public int? PrimaryImageId { get; set; }

        public string PrimaryImageUrl { get; set; }

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
