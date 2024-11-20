using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CameraRentalApp.Data;
using CameraRentalApp.Models;
using CameraRentalApp.Services;
using System.Threading.Tasks;

public class CustomerController : Controller
{
    private readonly CustomerService _customerService;

    public CustomerController( CustomerService customerService)
    {
        _customerService = customerService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5, string searchTerm = "")
    {
        var customers = await _customerService.GetAllCustomerAsync();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            customers = customers.Where(c => c.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                              c.Email.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                              c.Address.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                              c.PhoneNumber.ToString().Contains(searchTerm))
                                 .ToList();
        }

        var totalItems = customers.Count;

        var paginatedCustomers = customers
            .OrderBy(c => c.CustomerName)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

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
            await _customerService.CreateCustomerAsync(customer);
            TempData["Message"] = "Success Added Product!";
            return RedirectToAction("Index");
            //return RedirectToAction("CreateTransaction", new { customerId = customer.CustomerId });
        }
        return View(customer);
    }

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
        await _customerService.DeleteCustomerAsync(customerId);
        TempData["Message"] = "Customer deleted successfully.";
        return RedirectToAction("Index");
    }
}





