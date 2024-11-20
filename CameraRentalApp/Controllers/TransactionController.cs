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

        // GET: Transaction/Index
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


    /*    // GET: Transaction/Index
        [HttpGet]
        public async Task<IActionResult> TransactionHistory(int pageNumber = 1, int pageSize = 5, string searchTerm = "")
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
        }*/

        // GET: Transaction/Create
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

        // POST: Transaction/Create
        [HttpPost]
        public async Task<IActionResult> Create(TransactionViewModel viewModel)
        {
            if (viewModel.Transaction == null)
            {
                viewModel.Transaction = new Transaction();
            }
            
            _logger.LogInformation("CustomerId (dari form): {CustomerId}", viewModel.Transaction.CustomerId);
            _logger.LogInformation("CameraId (dari form): {CameraId}", viewModel.Transaction.CameraId);

            var customer = await _customerService.GetCustomerByIdAsync(viewModel.Transaction.CustomerId);
            var camera = await _cameraService.GetCameraByIdAsync(viewModel.Transaction.CameraId);

            if (customer == null || camera == null)
            {
                ModelState.AddModelError("", "Invalid Customer or Camera selected.");
            }
            else
            {
                viewModel.Transaction.Customer = customer;
                viewModel.Transaction.Camera = camera;
            }

            if (!ModelState.IsValid)
            {
                ViewBag.CustomerId = viewModel.Transaction.CustomerId;
                ViewBag.CameraId = viewModel.Transaction.CameraId;

                _logger.LogInformation("Transaction: {Transaction}", JsonConvert.SerializeObject(viewModel.Transaction));

                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    _logger.LogError("Validation error: {Error}", error.ErrorMessage);
                }

                viewModel.Customers = await _customerService.GetAllCustomerAsync();
                viewModel.Cameras = await _cameraService.GetAllCameraAsync();

               return View(viewModel);
            }

            try
            {
                await _transactionService.AddTransactionAsync(viewModel.Transaction);

                TempData["SuccessMessage"] = "Transaction created successfully!";
                return RedirectToAction("Index");
            }
            catch (ArgumentException ex)
            {
                ModelState.AddModelError("", ex.Message);
                TempData["ErrorMessage"] = "Failed to create transaction: " + ex.Message;

                _logger.LogError("Error occurred while creating transaction: {Message}", ex.Message);

                viewModel.Customers = await _customerService.GetAllCustomerAsync();
                viewModel.Cameras = await _cameraService.GetAllCameraAsync();

                return View(viewModel);
            }
        }


        /*   // GET: CreateCustomer Partial View
           [HttpGet]
               public IActionResult CreateCustomer()
               {
                   return PartialView("_CreateCustomer");
               }*/

        /*[Route("Transaction/AddCustomer")]
        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] TransactionViewModel model)
        {
            if (model == null || model.Transaction?.Customer == null || !ModelState.IsValid)
            {
                return Json(new { success = false, message = "Invalid customer data." });
            }

            try
            {
                // Konversi dari TransactionViewModel ke Customer
                var newCustomer = new Customer
                {
                    CustomerName = model.Transaction.Customer.CustomerName,
                    PhoneNumber = model.Transaction.Customer.PhoneNumber,
                    Email = model.Transaction.Customer.Email,
                    Address = model.Transaction.Customer.Address
                };

                // Panggil metode service dengan Customer
                var addedCustomer = await _transactionService.AddCustomerAsync(newCustomer);

                return Json(new { success = true, customer = addedCustomer });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }*/

        [Route("Transaction/AddCustomer")]
        [HttpPost]
        public async Task<IActionResult> AddCustomer([FromBody] TransactionViewModel model)
        {
            return RedirectToAction(nameof(TransactionHistory));
        }




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
