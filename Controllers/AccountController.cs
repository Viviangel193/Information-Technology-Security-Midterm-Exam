using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using SampleSecureWeb.Data;
using SampleSecureWeb.Models;
using SampleSecureWeb.Services;
using SampleSecureWeb.ViewModel;

namespace SampleSecureWeb.Controllers
{
    public class AccountController : Controller
    {
        private const int MaxFailedAttempts = 5;
        private const int LockoutDurationInMinutes = 1;

        private readonly IUser _userData;
        private readonly IMemoryCache _cache;
        private readonly IUser _userService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly PasswordSuggestionService _passwordSuggestionService;

        public AccountController(IUser userData, IMemoryCache memoryCache, IUser userService,
                                 UserManager<User> userManager, SignInManager<User> signInManager,
                                 RoleManager<IdentityRole> roleManager, PasswordSuggestionService passwordSuggestionService)
        {
            _userData = userData;
            _cache = memoryCache;
            _userService = userService;
            _userManager = userManager;
            _signInManager = signInManager;
            _roleManager = roleManager;
            _passwordSuggestionService = passwordSuggestionService;
        }

        [AllowAnonymous]
        public IActionResult Register()
        {
            ViewBag.PasswordSuggestion = _passwordSuggestionService.GenerateStrongPassword();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Register(RegistrationViewModel model)
        {
            if (ModelState.IsValid)
            {
                if (!PasswordValidator.VerifyPassword(model.Password))
                {
                    ModelState.AddModelError(string.Empty, "Password must contain only printable Unicode characters.");
                    return View(model);
                }

                var user = new User
                {
                    UserName = model.UserName,
                    Email = model.Email,
                };

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    if (!await _roleManager.RoleExistsAsync("DefaultRole"))
                    {
                        await _roleManager.CreateAsync(new IdentityRole("DefaultRole"));
                    }
                    await _userManager.AddToRoleAsync(user, "DefaultRole");
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }

            ViewBag.PasswordSuggestion = _passwordSuggestionService.GenerateStrongPassword();
            return View(model);
        }

        [HttpGet]
        [Route("Account/Login")]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
[Route("Account/Login")]
[AllowAnonymous]
public async Task<IActionResult> Login(LoginViewModel loginViewModel)
{
    if (ModelState.IsValid)
    {
        // Mendapatkan IP Address pengguna
        var userIp = HttpContext.Connection.RemoteIpAddress?.ToString();
        if (string.IsNullOrEmpty(userIp))
        {
            ModelState.AddModelError(string.Empty, "Tidak dapat menentukan alamat IP. Coba lagi nanti.");
            return View(loginViewModel);
        }

        // Menggabungkan UserName dan IP Address sebagai cacheKey unik
        var cacheKey = $"FailedAttempts_{loginViewModel.UserName}_{userIp}";
        var lockoutKey = $"LockoutTime_{loginViewModel.UserName}_{userIp}";

        // Cek percobaan login yang gagal
        if (!_cache.TryGetValue(cacheKey, out int failedAttempts))
        {
            failedAttempts = 0;
        }

        // Cek apakah pengguna sedang dalam masa pemblokiran
        if (_cache.TryGetValue(lockoutKey, out DateTime lockoutEndTime) && DateTime.Now < lockoutEndTime)
        {
            ModelState.AddModelError(string.Empty, "Akun Anda terkunci sementara. Silakan coba lagi setelah 1 menit.");
            return View(loginViewModel);
        }

        var result = await _signInManager.PasswordSignInAsync(
            loginViewModel.UserName, 
            loginViewModel.Password, 
            isPersistent: false, 
            lockoutOnFailure: false
        );

        if (result.Succeeded)
        {
            // Reset percobaan gagal dan waktu blokir jika login berhasil
            _cache.Remove(cacheKey);
            _cache.Remove(lockoutKey);
            return RedirectToAction("Index", "Home");
        }
        else
        {
            // Increment percobaan gagal
            failedAttempts++;
            _cache.Set(cacheKey, failedAttempts, TimeSpan.FromMinutes(60)); // Atur waktu penyimpanan cache

            // Jika percobaan gagal mencapai batas, atur waktu blokir
            if (failedAttempts >= MaxFailedAttempts)
            {
                _cache.Set(lockoutKey, DateTime.Now.AddMinutes(LockoutDurationInMinutes), TimeSpan.FromMinutes(LockoutDurationInMinutes));
                _cache.Remove(cacheKey); // Reset percobaan gagal setelah blokir aktif
                ModelState.AddModelError(string.Empty, "Anda telah gagal login 5 kali. Silakan coba lagi setelah 1 menit.");
            }
            else
            {
                ModelState.AddModelError(string.Empty, $"Login gagal. Kesempatan login tersisa: {MaxFailedAttempts - failedAttempts}");
            }
        }
    }
    return View(loginViewModel);
}


        private IActionResult RedirectToLocal(string? returnUrl)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel changePasswordViewModel)
        {
            if (ModelState.IsValid)
            {
                if (!PasswordValidator.VerifyPassword(changePasswordViewModel.NewPassword))
                {
                    ModelState.AddModelError(string.Empty, "New password must contain only printable Unicode characters.");
                    return View(changePasswordViewModel);
                }

                var user = await _userManager.GetUserAsync(User);
                var result = await _userManager.ChangePasswordAsync(user, changePasswordViewModel.CurrentPassword, changePasswordViewModel.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            return View(changePasswordViewModel);
        }

        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}