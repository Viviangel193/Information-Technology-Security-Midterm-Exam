using System.Threading.Tasks;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Data
{
    public class UserRepository : IUser
    {
        private readonly ApplicationDbContext _context;

        // Constructor
        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User> Registration(User user)
        {
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
            _context.Users.Add(user);  // Tambahkan pengguna ke konteks
            await _context.SaveChangesAsync();  // Simpan perubahan ke database
            return user;
        }

        public async Task<User> Login(User user)
        {
            // Implement login logic here
            return user;
        }

        public async Task ChangePassword(string username, string currentPassword, string newPassword)
        {
            // Logic for changing the password
        }

        public User ValidateUser(string username, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.UserName == username);
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                return user; // User found and password matches
            }
            return null; // User not found or password does not match
        }
    }
}
