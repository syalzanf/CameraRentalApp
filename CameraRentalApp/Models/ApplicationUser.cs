using Microsoft.AspNetCore.Identity;

namespace CameraRentalApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        public string Role { get; set; } // Menyimpan peran pengguna (Admin atau Superadmin)
        public string FullName { get; set; }
    }
}
