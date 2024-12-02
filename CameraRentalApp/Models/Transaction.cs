using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace CameraRentalApp.Models
{
    public class Transaction
    {
        [Key]
        public int RentalId { get; set; }

        [Required]
        public string RentalCode { get; set; }

        [Required]
        public int CustomerId { get; set; }

        [Required]
        public int CameraId { get; set; }

        public DateTime RentalDate { get; set; } = DateTime.Now;
        public DateTime ReturnDate { get; set; } = DateTime.Now;

        public decimal RentalPricePerDay { get; set; }
        public decimal TotalPrice { get; set; }
        public decimal TotalPay { get; set; }
        public decimal Change { get; set; }

        public string PaymentMethod { get; set; }
        public string? Status { get; set; }
        public string? return_proof { get; set; }

        [ValidateNever]
        public Customer? Customer { get; set; }

        [ValidateNever]
        public Camera? Camera { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
