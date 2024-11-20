using CameraRentalApp.Data;
using CameraRentalApp.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO; 
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CameraRentalApp.Services
{
    public class TransactionService
    {
        private readonly ApplicationDbContext _context;
        private readonly string _uploadDirectory;


        public TransactionService(ApplicationDbContext context)
        {
            _context = context;
            _uploadDirectory = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");
        }


        /*   public async Task AddCustomerAsync(Customer customer)
           {
               if (customer == null) throw new ArgumentNullException(nameof(customer));

               await _context.Customers.AddAsync(customer);
               await _context.SaveChangesAsync();
           }*/

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            if (customer == null)
            {
                throw new ArgumentException("Invalid customer");
            }

            _context.Customers.Add(customer);
            await _context.SaveChangesAsync();

            return customer;
        }


    public async Task AddTransactionAsync(Transaction transaction)

        {

            int rentalDays = (transaction.ReturnDate - transaction.RentalDate).Days;

            if (rentalDays > 0)
            {
                transaction.TotalPrice = rentalDays * transaction.RentalPricePerDay;
            }
            else
            {
                throw new ArgumentException("Return date must be after rental date.");
            }

            if (transaction.PaymentMethod != "cash")
            {
                transaction.TotalPay = transaction.TotalPrice;
            }

            transaction.Status = "Ongoing";

            
            await _context.Transactions.AddAsync(transaction);
            await _context.SaveChangesAsync();
        }

        public IQueryable<Transaction> GetAllTransactionsAsync()
        {
            return _context.Transactions.AsQueryable();
        }


        public async Task<string> ProcessPaymentAsync(int transactionId, decimal totalPayment)
        {
            var transaction = await _context.Transactions.FindAsync(transactionId);
            if (transaction == null)
            {
                throw new InvalidOperationException("Transaction not found.");
            }

            if (transaction.PaymentMethod == "cash")
            {


                if (totalPayment < transaction.TotalPrice)
                {
                    throw new ArgumentException("Insufficient payment.");
                }

                transaction.TotalPay = totalPayment;

                transaction.Change = totalPayment - transaction.TotalPrice;

                if (transaction.ReturnDate < DateTime.Now)
                {
                    transaction.Status = "Overdue"; 
                }
                else
                {
                    transaction.Status = "Confirmed"; 
                }
            }
            else
            {
                transaction.TotalPay = transaction.TotalPrice;
            }

            _context.Update(transaction);
            await _context.SaveChangesAsync();

            return $"Payment successful. Change amount: {transaction.Change}";
        }

        public async Task<string> UploadReturnProofAsync(int transactionId, IFormFile returnProof)
        {
            if (returnProof != null && returnProof.Length > 0)
            {

                var filePath = Path.Combine(_uploadDirectory, returnProof.FileName);

                using (var stream = new FileStream(filePath, FileMode.Create)) 
                {
                    await returnProof.CopyToAsync(stream);
                }

                var transaction = await _context.Transactions.FindAsync(transactionId);
                if (transaction != null)
                {

                    if (transaction.Status == "Returned")
                    {
                      
                        transaction.return_proof = $"/uploads/{returnProof.FileName}";

                        if (transaction.ReturnDate.Date == DateTime.Now.Date)
                        {
                            transaction.Status = "Confirmed";  
                        }
                        else if (transaction.ReturnDate < DateTime.Now.Date)
                        {
                            
                            transaction.Status = "Overdue";
                        }

                        _context.Update(transaction);
                        await _context.SaveChangesAsync();

                        return transaction.return_proof;
                    }
                    else
                    {
                        throw new InvalidOperationException("Transaction must be marked as 'Returned' before uploading proof.");
                    }
                }
            }

            return null;
        }


        public async Task CheckAndUpdateTransactionStatusesAsync()
        {   
            var now = DateTime.Now;

            var transactionsToReturn = await _context.Transactions
                .Where(t => t.Status == "Ongoing" && t.ReturnDate <= now)
                .ToListAsync();

            foreach (var transaction in transactionsToReturn)
            {
                transaction.Status = "Returned";
            }

            await _context.SaveChangesAsync();
        }
    }
}