using SampleSecureWeb.Models;
using System.Threading.Tasks;

namespace SampleSecureWeb.Data
{
    public interface IUser
    {
        Task<User> Registration(User user); // Registration
        Task<User> Login(User user); // Login
        Task ChangePassword(string username, string currentPassword, string newPassword); // Change Password
        User ValidateUser(string username, string password); // Validate User
    }
}
