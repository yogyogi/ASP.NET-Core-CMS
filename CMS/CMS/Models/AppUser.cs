using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace CMS.Models
{
    public class AppUser : IdentityUser
    {
        public Country Country { get; set; }

        public int Age { get; set; }
    }

    public enum Country
    {
        Belgium, USA, England, France, Germany, Russia, Other
    }
}
