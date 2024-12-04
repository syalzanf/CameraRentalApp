/*using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace CameraRentalApp.Controllers
{
    public class HomeController : Controller
    {
        [Route("")]
        public IActionResult Index()
        {
            // Mengecek apakah pengguna sudah login
            if (User.Identity.IsAuthenticated)
            {
                // Mengambil role pengguna dari klaim
                var role = User.FindFirst(System.Security.Claims.ClaimTypes.Role)?.Value;

                if (role == "Admin")
                {
                    return RedirectToAction("AdminDashboard", "Account");  // Redirect ke dashboard Admin
                }
                else if (role == "SuperAdmin")
                {
                    return RedirectToAction("SuperAdminDashboard", "Account");  // Redirect ke dashboard SuperAdmin
                }
                else
                {
                    return RedirectToAction("Index", "Home");  // Redirect ke halaman beranda jika role tidak dikenali
                }
            }

            // Jika pengguna belum login, tampilkan halaman beranda tanpa navbar
            ViewData["HideNavbar"] = "true";
            return View();  // Menampilkan halaman utama aplikasi
        }
    }
}*/





using CameraRentalApp.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace CameraRentalApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
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