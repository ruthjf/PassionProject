using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace PassionProject.Models
{
    public class AppUser
    {
        [Key]
        public int AppUserID { get; set; }
        public string AppUsername { get; set; }
        public string AppUserFirstName { get; set; }
        public string AppUserLastName { get; set; }
      
    }
}