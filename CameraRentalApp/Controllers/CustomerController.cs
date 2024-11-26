using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CameraRentalApp.Data;
using CameraRentalApp.Models;
using CameraRentalApp.Services;
using System.Threading.Tasks;

public class CustomerController : Controller
{
    private readonly CustomerService _customerService;
    private readonly ILogger<CustomerController> _logger;

    public CustomerController( CustomerService customerService, ILogger<CustomerController> logger)
    {
        _customerService = customerService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5, string searchTerm = "")
    {
        var customers = await _customerService.GetAllCustomerAsync();
        _logger.LogInformation("Total customers retrieved: {Count}", customers.Count);


        if (!string.IsNullOrEmpty(searchTerm))
        {
            customers = customers.Where(c => c.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                              c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                              c.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                              c.PhoneNumber.ToString().Contains(searchTerm))
                                 .ToList();
            _logger.LogInformation("Customers after filtering: {Count}", customers.Count);

        }

        var totalItems = customers.Count;
        _logger.LogInformation("Paginating data. Total items: {TotalItems}, PageNumber: {PageNumber}, PageSize: {PageSize}", totalItems, pageNumber, pageSize);


        var paginatedCustomers = customers
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        _logger.LogInformation("Customers to display on current page: {Count}", paginatedCustomers.Count);

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.SearchTerm = searchTerm;

        return View(paginatedCustomers); 
    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Customer customer)
    {
        if (ModelState.IsValid)
        {
            await _customerService.AddCustomerAsync(customer);
            TempData["Message"] = "Success Added Product!";
            return RedirectToAction("Index");
            //return RedirectToAction("CreateTransaction", new { customerId = customer.CustomerId });
        }
        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(int id)
    {
        var customer = await _customerService.GetCustomerByIdAsync(id);
        if (customer == null)
        {
            return NotFound();
        }
        return View(customer);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Customer customer)
    {
        if (ModelState.IsValid)
        {
            await _customerService.UpdateCustomerAsync(customer);
            TempData["Message"] = "Success Updated Product!";
            return RedirectToAction("Index");
        }
        return View(customer);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int customerId)
    {
        _logger.LogInformation($"Delete action for customer ID: {customerId}");

        bool isDeleted = await _customerService.DeleteCustomerAsync(customerId);
        if (!isDeleted)
        {
            _logger.LogWarning($"Delete failed: Customer ID {customerId} not found.");
            TempData["Error"] = "Customer not found!";
        }
        else
        {
            _logger.LogInformation($"Customer ID {customerId} deleted successfully.");
            TempData["Message"] = "Customer deleted successfully!";
        }

        return RedirectToAction("Index");
    }
}





