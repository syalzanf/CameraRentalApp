using System.ComponentModel.DataAnnotations;


namespace CameraRentalApp.Models
{
    public class Camera
    {
        [Key]
        public int CameraId { get; set; }

 
        public string CameraCode { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Brand { get; set; }
        
        public string Image { get; set; }

        public decimal RentalPricePerDay { get; set; }
        public int Stock { get; set; }
        public bool IsDeleted { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        //public IFormFile? ImageFile { get; set; }

    }
}
