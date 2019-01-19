using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CMS.Models
{
    public partial class Menu
    {
        public int Id { get; set; }
        [Required]
        public string Name { get; set; }
        public string Item { get; set; }
        public DateTime AddedOn { get; set; }
        [Required]
        public bool Status { get; set; }
    }
}
