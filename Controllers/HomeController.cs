using System.Diagnostics;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SampleSecureWeb.Models;

namespace SampleSecureWeb.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string username = User.Identity.IsAuthenticated ? User.Identity.Name : "Guest";
            ViewBag.username = username;

            string[] fruits = new string[] { "Angela Listya P.K / 72220521", "Tiffany Aurelia S / 72220541", "Vivian / 72220590" };
            ViewBag.fruits = fruits;

            return View();
        }

        public IActionResult About()
        {
            ViewData["Title"] = "About Page";
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
