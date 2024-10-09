using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Data
{
    public interface IUser
    {
        User Registration(User user);  // Pastikan tipe pengembalian adalah void
        User Login(User user);
        void ChangePassword(string username, string currentPassword, string newPassword);
    }
}
