using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace SampleSecureWeb.Models
{
    public class User : IdentityUser // Pastikan User mewarisi dari IdentityUser
    {
        [Key]
        public int Id { get; set; } // Primary key

        public string RoleName { get; set; } = string.Empty; // Atur default untuk menghindari NULL
    }
}
