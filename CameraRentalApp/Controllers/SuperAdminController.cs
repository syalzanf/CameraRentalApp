using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CameraRentalApp.Controllers
{
    [Authorize(Roles = "Superadmin")]
    [Route("SuperAdmin")]
    public class SuperAdminController : Controller
    {
        [HttpGet("dashboard")]
        public IActionResult SuperAdminDashboard()
        {
            return View(); 
        }
    }
}
