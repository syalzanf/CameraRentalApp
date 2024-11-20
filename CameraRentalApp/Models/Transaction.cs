using System.ComponentModel.DataAnnotations;

namespace CameraRentalApp.Models
{
    public class Transaction
    {
        [Key]
        public int RentalId { get; set; }

        [Required(ErrorMessage = "Customer field is required.")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Camera field is required.")]
        public int CameraId { get; set; }

        public DateTime RentalDate { get; set; } = DateTime.Now;
        public DateTime ReturnDate { get; set; } = DateTime.Now;

        public decimal RentalPricePerDay { get; set; }
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; }
        public decimal TotalPay { get; set; }
        public decimal Change { get; set; }


        public string? Status { get; set; }
        public string? return_proof { get; set; }


        public Customer? Customer { get; set; }
        public Camera? Camera { get; set; }

    }
}
