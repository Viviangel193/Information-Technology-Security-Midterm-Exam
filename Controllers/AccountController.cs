using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using SampleSecureWeb.Data;
using SampleSecureWeb.Models;
using SampleSecureWeb.ViewModel;

namespace SampleSecureWeb.Controllers
{
    public class AccountController : Controller
    {
        private readonly IUser _userData;
        
        public AccountController(IUser user)
        {
            _userData = user;
        }
        
        
        // GET: AccountController
        public IActionResult Index()
        {
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(RegistrationViewModel registrationViewModel)
        {
            try
            {
                if(ModelState.IsValid)
                {
                    var user = new Models.User
                    {
                        UserName = registrationViewModel.UserName,
                        Password = registrationViewModel.Password,
                        RoleName = "Contributor"
                    };
                    _userData.Registration(user);
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (System.Exception ex) 
            {
                ViewBag.Error = ex.Message;
            }
            return View(registrationViewModel);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
[HttpPost]
public async Task<ActionResult> Login(LoginViewModel loginViewModel)
{
    try
    {
        var user = new User
        {
            UserName = loginViewModel.UserName,
            Password = loginViewModel.Password
        };

        var loginUser = _userData.Login(user);
        if (loginUser == null)
        {
            ViewBag.Message = "Invalid Login attempt.";
            return View(loginViewModel);
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName)
        };

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = loginViewModel.RememberLogin
            });

        return RedirectToAction("Index", "Home");
    }
    catch (Exception ex)
    {
        ViewBag.Error = ex.Message;
    }
    return View(loginViewModel);
}


    public IActionResult ChangePassword()
{
    return View();
}

[HttpPost]
public ActionResult ChangePassword(ChangePasswordViewModel changePasswordViewModel)
{
    try
    {
        if (ModelState.IsValid)
        {
            _userData.ChangePassword(changePasswordViewModel.UserName, changePasswordViewModel.CurrentPassword, changePasswordViewModel.NewPassword);
            return RedirectToAction("Index", "Home");
        }
    }
    catch (Exception ex)
    {
        ViewBag.Error = ex.Message;
    }

    return View(changePasswordViewModel);
}
    }}