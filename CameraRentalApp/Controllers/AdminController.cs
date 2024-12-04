using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


namespace CameraRentalApp.Controllers
{
    [Authorize(Roles = "Admin")]
    [Route("admin")]
    public class AdminController : Controller
    {
        [Route("dashboard")]
        public IActionResult AdminDashboard()
        {
            return View();
        }

        [Route("customers")]
        public IActionResult Customers()
        {
            return RedirectToAction("Index", "Customer");
        }

        [Route("cameras")]
        public IActionResult Cameras()
        {
            return RedirectToAction("Index", "Camera");
        }

        [Route("transactions")]
        public IActionResult Transactions()
        {
            return RedirectToAction("Index", "Transaction");
        }
    }
}