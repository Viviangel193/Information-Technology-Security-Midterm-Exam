using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Data
{
    public class UserData : IUser
    {
        private readonly ApplicationDbContext _db;

        public UserData(ApplicationDbContext db)
        {
            _db = db;
        }

        public async Task<User> Login(User user)
        {
            var _user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == user.UserName);
            if (_user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(user.PasswordHash, _user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Password is incorrect.");
            }

            return _user;
        }

        public async Task<User> Registration(User user)
        {
            ValidatePassword(user.PasswordHash); // Validate the password strength
            
            // Check if user already exists
            if (await _db.Users.AnyAsync(u => u.UserName == user.UserName))
            {
                throw new InvalidOperationException("User already exists.");
            }

            try
            {
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.PasswordHash);
                await _db.Users.AddAsync(user);
                await _db.SaveChangesAsync();
                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error during registration: {ex.Message}");
            }
        }

        public async Task ChangePassword(string username, string currentPassword, string newPassword)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            ValidatePassword(newPassword);
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
            await _db.SaveChangesAsync();
        }

        public async Task<User> ValidateUserAsync(string username, string password)
        {
            var user = await _db.Users.FirstOrDefaultAsync(u => u.UserName == username);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                throw new UnauthorizedAccessException("Password is incorrect.");
            }

            return user;
        }

        private void ValidatePassword(string password)
        {
            if (password.Length < 12 ||
                !password.Any(char.IsUpper) ||
                !password.Any(char.IsLower) ||
                !password.Any(char.IsDigit))
            {
                throw new ArgumentException("Password must be at least 12 characters long, contain uppercase letters, lowercase letters, and numbers.");
            }
        }

        public User ValidateUser(string username, string password)
        {
            throw new NotImplementedException();
        }
    }
}
