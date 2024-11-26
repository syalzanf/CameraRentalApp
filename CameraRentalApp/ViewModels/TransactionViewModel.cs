using CameraRentalApp.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;



namespace CameraRentalApp.ViewModels
{
    public class TransactionViewModel
    {
        public Transaction Transaction { get; set; }
        public Customer Customer { get; set; }

        public IEnumerable<Customer>? Customers { get; set; }
        public IEnumerable<Camera>? Cameras { get; set; }

    }
}
