using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Data
{
    public class UserRepository : IUser
    {
        // Anda mungkin ingin menggunakan konteks database di sini jika menggunakan Entity Framework
        // private readonly ApplicationDbContext _context;

        // Constructor
        public UserRepository(/* ApplicationDbContext context */)
        {
            // _context = context;
        }

        public User Registration(User user)
{
    // Implementasi logika pendaftaran pengguna
    // Misalnya: simpan pengguna ke dalam database
    // Gantilah kode ini dengan logika pendaftaran yang sesuai

    // Contoh sederhana:
    user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
    
    // Simpan pengguna ke database (contoh, jika menggunakan EF)
    // _context.Users.Add(user);
    // _context.SaveChanges();

    return user; // Mengembalikan objek pengguna yang telah didaftarkan
}

        public User Login(User user)
        {
            // Implementasi logika login pengguna
            // Misalnya: periksa kredensial pengguna di dalam database
            return user; // Ganti dengan logika yang sesuai
        }

        public void ChangePassword(string username, string currentPassword, string newPassword)
        {
            // Implementasi logika untuk mengubah password pengguna
            // Misalnya: periksa password saat ini dan perbarui dengan password baru
        }
    }
}