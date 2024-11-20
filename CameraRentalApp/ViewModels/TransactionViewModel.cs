using CameraRentalApp.Models;
using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;



namespace CameraRentalApp.ViewModels
{
    public class TransactionViewModel
    {
        public Transaction Transaction { get; set; }

        public IEnumerable<Customer>? Customers { get; set; }
        public IEnumerable<Camera>? Cameras { get; set; }

        /*[ValidateNever]
        public List<Customer> Customers { get; set; }

        [ValidateNever]
        public List<Camera> Cameras { get; set; }*/
    }
}
