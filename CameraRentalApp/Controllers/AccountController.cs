using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CameraRentalApp.Models;
using CameraRentalApp.ViewModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;


public class AccountController : Controller
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly UserManager<ApplicationUser> _userManager;

    public AccountController(SignInManager<ApplicationUser> signInManager,
                             UserManager<ApplicationUser> userManager)
    {
        _signInManager = signInManager;
        _userManager = userManager;
    }

    public IActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var user = await _userManager.FindByNameAsync(model.Username);
        if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password))
        {
            TempData["Error"] = "Invalid username or password!";
            return RedirectToAction("Login");
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, false, false);

        if (result.Succeeded)
        {
            var roles = await _userManager.GetRolesAsync(user);

            if (roles.Contains("Admin"))
            {
                return RedirectToAction("AdminDashboard", "Admin");
            }
            else if (roles.Contains("superadmin"))
            {
                return RedirectToAction("SuperAdminDashboard", "SuperAdmin");
            }
            else
            {
                TempData["Error"] = "Unknown role!";
                return RedirectToAction("Login");
            }
        }

        TempData["Error"] = "Login failed!";
        return RedirectToAction("Login");
    }

    [HttpPost]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();

        return RedirectToAction("Login");
    }


/*  [HttpGet]
    [AllowAnonymous]
    public async Task<IActionResult> Register()
    {
            var user = new ApplicationUser
            {
                UserName = "Admin",
                Email = "Admin@gmail.com",
                FullName = "Admin1",
                Role = "Admin"
            };

            var password = "Admin123@";

            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Admin");

                await _signInManager.SignInAsync(user, isPersistent: false);

            }
            else
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
        return RedirectToAction(nameof(Login));
    }*/
}





