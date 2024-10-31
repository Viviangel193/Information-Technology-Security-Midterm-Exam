using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SampleSecureWeb.Models
{
    public class UserProfile
    {
        [Key]
        public string UserId { get; set; }

        [Required(ErrorMessage = "Full Name is required.")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Email is required."), EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "Phone Number is required."), Phone]
        public string PhoneNumber { get; set; }
    }
}
