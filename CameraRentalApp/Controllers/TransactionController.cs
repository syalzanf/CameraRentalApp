using Microsoft.AspNetCore.Authorization;
using CameraRentalApp.Models;
using CameraRentalApp.Services;
using CameraRentalApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;



namespace CameraRentalApp.Controllers
    {
    [Authorize(Roles = "Admin")]
    public class TransactionController : Controller
        {
            private readonly TransactionService _transactionService;
            private readonly CameraService _cameraService;
            private readonly CustomerService _customerService;
            private readonly ILogger<TransactionController> _logger;

            public TransactionController(TransactionService transactionService, CameraService cameraService, CustomerService customerService, ILogger<TransactionController> logger)
        {
            _transactionService = transactionService;
            _cameraService = cameraService;
            _customerService = customerService;
            _logger = logger;
        }

        [Route("admin/transactions")]
        [HttpGet]
            public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5, string searchTerm = "")
            {
                var transactions = await _transactionService.GetAllTransactionsAsync()
                    .Include(t => t.Customer)
                    .Include(t => t.Camera)
                    .AsQueryable()
                    .ToListAsync();

                if (!string.IsNullOrEmpty(searchTerm))
                {
                    transactions = transactions
                        .Where(t => t.Customer.CustomerName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    t.RentalId.ToString().Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    t.Camera.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                    t.Status.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                }

                var totalItems = transactions.Count();
                var paginatedTransactions = transactions
                    .OrderBy(t => t.RentalId)
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToList();

                ViewBag.PageNumber = pageNumber;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalItems = totalItems;
                ViewBag.SearchTerm = searchTerm;


                return View(paginatedTransactions);
            }

        [Route("admin/transactions/history")]
        [HttpGet]
        public async Task<IActionResult> TransactionHistory(int pageNumber = 1, int pageSize = 5, string searchTerm = "")
        {
            var (transactions, totalItems) = await _transactionService.GetTransactionsAsync(pageNumber, pageSize, searchTerm);

            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalItems = totalItems;
            ViewBag.SearchTerm = searchTerm;

            return View(transactions);
        }

        [Route("admin/transactions/detail")]
        [HttpGet]
        public async Task<IActionResult> GetDetail(int rentalId)
        {
            var transaction = await _transactionService.GetTransactionDetailAsync(rentalId);

            if (transaction == null)
            { 
                return NotFound(new { message = "Transaction not found" });
            }

            return Json(new
            {
                rentalId = transaction.RentalId,
                customerName = transaction.Customer.CustomerName,
                cameraName = transaction.Camera.Name,
                rentalDate = transaction.RentalDate.ToString("yyyy-MM-dd"),
                returnDate = transaction.ReturnDate.ToString("yyyy-MM-dd")
            });
        }

        [Route("admin/transactions/create")]
        [HttpGet]
            public async Task<IActionResult> Create()
            {
                var cameras = await _cameraService.GetAllCameraAsync();
                var customers = await _customerService.GetAllCustomerAsync();

                var viewModel = new TransactionViewModel
                {
                    Transaction = new Transaction(),
                    Cameras = cameras,
                    Customers = customers
                };

                return View(viewModel);  
            }

        [Route("admin/transactions/create")]
        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel viewModel)
        {
            _logger.LogInformation("CustomerId (dari form): {CustomerId}", viewModel.Transaction.CustomerId);
            _logger.LogInformation("CameraId (dari form): {CameraId}", viewModel.Transaction.CameraId);

            var customer = await _customerService.GetCustomerByIdAsync(viewModel.Transaction.CustomerId);
            var camera = await _cameraService.GetCameraByIdAsync(viewModel.Transaction.CameraId);

            if (customer == null)
            {
                ModelState.AddModelError("Customer", "Customer not found.");
                _logger.LogError("Customer not found with ID: {CustomerId}", viewModel.Transaction.CustomerId);
            }

            if (camera == null)
            {
                ModelState.AddModelError("Camera", "Camera not found.");
                _logger.LogError("Camera not found with ID: {CameraId}", viewModel.Transaction.CameraId);
            }

            viewModel.Transaction.Customer = customer;
            viewModel.Transaction.Camera = camera;
            _logger.LogInformation("Customer setelah di-set: {Customer}", JsonConvert.SerializeObject(viewModel.Transaction.Customer));
            _logger.LogInformation("Camera setelah di-set: {Camera}", JsonConvert.SerializeObject(viewModel.Transaction.Camera));

            /*if (!ModelState.IsValid)
            {
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Validation error: {Error}", error.ErrorMessage);
                }

                viewModel.Customers = await _customerService.GetAllCustomerAsync();
                viewModel.Cameras = await _cameraService.GetAllCameraAsync();
                return View(viewModel);
            }*/

            try
            {
                await _transactionService.AddTransactionAsync(viewModel.Transaction);
                TempData["SuccessMessage"] = "Transaction created successfully!";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogError("Error creating transaction: {Message}", ex.Message);

                viewModel.Customers = await _customerService.GetAllCustomerAsync();
                viewModel.Cameras = await _cameraService.GetAllCameraAsync();
                return View(viewModel);
            }
        }

        [Route("admin/transactions/addCustomer")]
        [HttpPost]
        public async Task<IActionResult> AddCustomer(Customer customer)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest("Invalid customer data");
            }

            await _customerService.AddCustomerAsync(customer);

            return RedirectToAction(nameof(Create));
        }


        [Route("admin/uploadReturnProof")]
        [HttpPost]
        public async Task<IActionResult> UploadReturnProof(int transactionId, IFormFile returnProof)
        {
            if (returnProof != null)
            {
                try
                {
                    var result = await _transactionService.UploadReturnProofAsync(transactionId, returnProof);
                    TempData["Message"] = "Return proof uploaded and transaction confirmed successfully!";
                }
                catch (InvalidOperationException ex)
                {
                    TempData["Error"] = ex.Message;
                }
                catch (Exception)
                {
                    TempData["Error"] = "Failed to upload return proof.";   
                }
            }   
            else
            {
                TempData["Error"] = "No file selected for upload.";
            }

            return RedirectToAction("Index");
        }
    }
}
