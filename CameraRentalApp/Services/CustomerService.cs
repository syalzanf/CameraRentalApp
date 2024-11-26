using CameraRentalApp.Data;
using CameraRentalApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;


namespace CameraRentalApp.Services
{
    public class CustomerService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CustomerService> _logger;


        public CustomerService(ApplicationDbContext context, ILogger<CustomerService> logger)
        {
            _context = context;
            _logger = logger;

        }

        public async Task<List<Customer>> GetAllCustomerAsync()
        {
            return await _context.Customers
                                 .Where(c => !c.IsDeleted)
                                 .OrderByDescending(c => c.CreatedAt) 
                                 .ToListAsync();
        }


        public async Task<Customer> GetCustomerByIdAsync(int customerId)
        {
            return await _context.Customers.FindAsync(customerId);
        }

        public async Task<Customer> AddCustomerAsync(Customer customer)
        {
            if (await _context.Customers.AnyAsync(c => c.Email == customer.Email && !c.IsDeleted))
            {
                throw new InvalidOperationException("A customer with the same email already exists.");
            }

            _context.Customers.Add(customer);

            await _context.SaveChangesAsync();

            return customer;
        }


        public async Task UpdateCustomerAsync(Customer customer)
        {
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteCustomerAsync(int customerId)
        {
            _logger.LogInformation($"Attempting to delete customer with ID: {customerId}");


            var customer = await GetCustomerByIdAsync(customerId);
            if (customer == null)
            {
                _logger.LogWarning($"Customer with ID: {customerId} not found.");
                return false;
            }
            customer.IsDeleted = true;
            _context.Customers.Update(customer);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Customer with ID: {customerId} marked as deleted.");
            return true;
        }
    }
}
