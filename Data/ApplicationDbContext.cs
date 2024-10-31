using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Data
{
    public class ApplicationDbContext : IdentityDbContext<User> // Menggunakan User sebagai IdentityUser
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
       
        public DbSet<Student> Students { get; set; } = null!;
        public DbSet<UserProfile> UserProfiles { get; set; } = null!;
    }
}
