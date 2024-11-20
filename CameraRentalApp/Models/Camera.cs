using System.ComponentModel.DataAnnotations;


namespace CameraRentalApp.Models
{
    public class Camera
    {
        [Key]
        public int CameraId { get; set; }


        public string Name { get; set; }
        public string Brand { get; set; }
        public string RentalPricePerDay { get; set; }
        public int Stock { get; set; }
        public string Image { get; set; }
        public bool IsDeleted { get; set; }
    }
}
