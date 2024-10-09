using System;
using System.Linq;
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

        public User Login(User user)
        {
            var _user = _db.Users.FirstOrDefault(u => u.UserName == user.UserName);
            if (_user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(user.Password, _user.Password))
            {
                throw new UnauthorizedAccessException("Password is incorrect.");
            }

            return _user;
        }

        public User Registration(User user)
        {
            // Validasi password
            ValidatePassword(user.Password);

            try
            {
                user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
                _db.Users.Add(user);
                _db.SaveChanges();
                return user;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Error during registration: {ex.Message}");
            }
        }

        public void ChangePassword(string username, string currentPassword, string newPassword)
        {
            var user = _db.Users.FirstOrDefault(u => u.UserName == username);
            if (user == null)
            {
                throw new InvalidOperationException("User not found.");
            }

            if (!BCrypt.Net.BCrypt.Verify(currentPassword, user.Password))
            {
                throw new UnauthorizedAccessException("Current password is incorrect.");
            }

            // Validasi password baru
            ValidatePassword(newPassword);

            user.Password = BCrypt.Net.BCrypt.HashPassword(newPassword);
            _db.SaveChanges();
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
    }
}
