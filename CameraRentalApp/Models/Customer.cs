using System.ComponentModel.DataAnnotations;


namespace CameraRentalApp.Models
{
    public class Customer
    {
        [Key]
        public int CustomerId { get; set; }

        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }

        public bool IsDeleted { get; set; } 

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
