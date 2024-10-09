using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleSecureWeb.Models
{
    public class User
    {
        [Key]
        
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string RoleName { get; set; } = null!;   
    }
}