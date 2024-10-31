using System.Security.Cryptography;
using System.Text;
using SampleSecureWeb.Data;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Services
{
    public interface IUserService
    {
        Task<bool> Register(User user, string password);
        Task<User?> Login(string username, string password);
        bool ValidatePassword(string password, string passwordHash);
    }

    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;

        public UserService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<bool> Register(User user, string password)
        {
            // Hash password sebelum disimpan
            user.PasswordHash = HashPassword(password);
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<User?> Login(string username, string password)
        {
            var user = await _context.Users.FindAsync(username);
            if (user == null || !ValidatePassword(password, user.PasswordHash))
            {
                return null; // Invalid username or password
            }
            return user;
        }

        public bool ValidatePassword(string password, string passwordHash)
        {
            // Bandingkan password yang diberikan dengan hash
            return HashPassword(password) == passwordHash;
        }

        private string HashPassword(string password)
        {
            using (var hmac = new HMACSHA256())
            {
                var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password));
                return Convert.ToBase64String(hash);
            }
        }
    }
}
