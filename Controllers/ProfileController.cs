using Microsoft.AspNetCore.Mvc;
using SampleSecureWeb.Models;
using SampleSecureWeb.Data;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace SampleSecureWeb.Controllers
{
    [Authorize] // Hanya pengguna yang sudah login yang bisa mengakses
    public class ProfileController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProfileController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> Index()
        {
            var userId = User?.Identity?.Name;
            if (string.IsNullOrEmpty(userId))
            {
                return RedirectToAction("Login", "Account");
            }

            var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

            if (userProfile == null)
            {
                // Berikan nilai default jika PhoneNumber kosong
                userProfile = new UserProfile
                {
                    UserId = userId,
                    FullName = "Default FullName", // Tambahkan nilai default untuk FullName
                    Email = "default@example.com", // Berikan nilai default untuk Email
                    PhoneNumber = "000-000-0000" // Berikan nilai default untuk PhoneNumber
                };
                _context.UserProfiles.Add(userProfile);
                await _context.SaveChangesAsync();
            }

            return View(userProfile);
        }

        [HttpPost]
        public async Task<IActionResult> Update(UserProfile model)
        {
            if (ModelState.IsValid)
            {
                var userId = User?.Identity?.Name;
                if (string.IsNullOrEmpty(userId))
                {
                    return RedirectToAction("Login", "Account");
                }

                var userProfile = await _context.UserProfiles.FirstOrDefaultAsync(up => up.UserId == userId);

                if (userProfile != null)
                {
                    userProfile.FullName = string.IsNullOrEmpty(model.FullName) ? "Default FullName" : model.FullName;
                    userProfile.Email = string.IsNullOrEmpty(model.Email) ? "default@example.com" : model.Email;
                    userProfile.PhoneNumber = string.IsNullOrEmpty(model.PhoneNumber) ? "000-000-0000" : model.PhoneNumber; // Berikan nilai default jika kosong

                    _context.UserProfiles.Update(userProfile);
                    await _context.SaveChangesAsync();

                    ViewBag.Message = "Profile updated successfully!";
                }
            }
            return View("Index", model);
        }
    }
}
