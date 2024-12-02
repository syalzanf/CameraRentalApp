using System.ComponentModel.DataAnnotations;


namespace CameraRentalApp.Models
{
    public class User
    {
        public int Id { get; set; }

        [Required]
        public string Username { get; set; }

        [Required]
        public string PasswordHash { get; set; }

        public string FullName { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public ICollection<UserRole> UserRoles { get; set; }
    }
}
