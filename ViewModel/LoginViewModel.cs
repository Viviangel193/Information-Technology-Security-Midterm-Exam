using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleSecureWeb.ViewModel
{
    public class LoginViewModel
    {
       [Required]
       public string UserName { get; set; } = string.Empty;

       [Required]
       [DataType(DataType.Password)]
       public string Password { get; set; } = string.Empty;

       public bool RememberLogin { get; set; }
       public string ReturnUrl { get; set; } = string.Empty; 
    }
}