using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CameraRentalApp.Data;
using CameraRentalApp.Models;
using CameraRentalApp.Services;
using System.Threading.Tasks;


public class CameraController : Controller
{
    private readonly CameraService _cameraService;
    private readonly ILogger<CameraController> _logger;


    public CameraController(ApplicationDbContext context, CameraService cameraService, ILogger<CameraController> logger)
    {
        _cameraService = cameraService;
        _logger = logger;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int pageNumber = 1, int pageSize = 5, string searchTerm = "")
    {
        var cameras = await _cameraService.GetAllCameraAsync();

        if (!string.IsNullOrEmpty(searchTerm))
        {
            cameras = cameras.Where(c => c.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         c.Brand.Contains(searchTerm, StringComparison.OrdinalIgnoreCase) ||
                                         c.RentalPricePerDay.ToString().Contains(searchTerm))
                             .ToList();
        }

        var totalItems = cameras.Count;


        var paginatedCameras = cameras
            .OrderByDescending(c => c.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.SearchTerm = searchTerm;


        return View(paginatedCameras);
    }

    [HttpGet]
    public async Task<IActionResult> Create()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> Create(Camera camera, IFormFile imageFile)
    {
        if (!ModelState.IsValid)
        {
            return View(camera);
        }

        try
        {
            if (imageFile != null && imageFile.Length > 0)
            {
                var allowedExtensions = new[] { ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();
                if (!allowedExtensions.Contains(fileExtension))
                {
                    TempData["Error"] = "Invalid file type. Only JPG, JPEG, PNG are allowed.";
                    return View(camera);
                }

                // Validasi ukuran file (maksimal 5MB)
                if (imageFile.Length > 5 * 1024 * 1024)
                {
                    TempData["Error"] = "File size exceeds the maximum allowed size of 5 MB.";
                    return View(camera);
                }
                var imagePath = await _cameraService.UploadImageAsync(imageFile);
                camera.Image = imagePath;
            }
            else
            {
                ModelState.AddModelError("Image", "Image file is required.");
                return View(camera);
            }
            await _cameraService.CreateCameraAsync(camera);

            TempData["Message"] = "Camera created successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError($"Error occurred while creating camera: {ex.Message}");
            ModelState.AddModelError("", "An error occurred while creating the camera.");
            return View(camera);
        }
    }

    /*    [HttpPost]
        public async Task<IActionResult> Create(Camera camera, IFormFile imageFile)
        {
            if (ModelState.IsValid)
            {
                if (imageFile != null && imageFile.Length > 0)
                {

                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                    var fileExtension = Path.GetExtension(imageFile.FileName).ToLower();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        TempData["Error"] = "Invalid file type. Only JPG, JPEG, PNG, and GIF are allowed.";
                        return View(camera);
                    }

                    if (imageFile.Length > 5 * 1024 * 1024)
                    {
                        TempData["Error"] = "File size exceeds the maximum allowed size of 5 MB.";
                        return View(camera);
                    }

                    var imagePath = await _cameraService.UploadImageAsync(imageFile);
                    camera.Image = imagePath;
                }

                await _cameraService.CreateCameraAsync(camera);
                TempData["Message"] = "Camera added successfully!";
                return RedirectToAction("Index");
            }

            TempData["Error"] = "There was an error adding the camera.";
            return View(camera);
        }*/


    public async Task<IActionResult> Edit(int id)
    {
        var camera = await _cameraService.GetCameraByIdAsync(id);
        if (camera == null)
        {
            return NotFound();
        }
        return View(camera);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Camera camera, IFormFile imageFile)
    {
        if (!ModelState.IsValid)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
            {
                _logger.LogError(error.ErrorMessage); // Menampilkan pesan error
            }

        try
        {
            var existingCamera = await _cameraService.GetCameraByIdAsync(camera.CameraId);
            if (existingCamera == null)
            {
                return NotFound();
            }

            // Cek apakah ada gambar baru yang di-upload
            if (imageFile != null && imageFile.Length > 0)
            {
                // Validasi file gambar
                if (imageFile.ContentType.StartsWith("image/"))
                {
                    // Hapus gambar lama jika ada
                    if (!string.IsNullOrEmpty(existingCamera.Image))
                    {
                        await _cameraService.DeleteImageAsync(existingCamera.Image);
                    }

                    // Upload gambar baru
                    var imagePath = await _cameraService.UploadImageAsync(imageFile);
                    camera.Image = imagePath;  // Set gambar baru
                }
                else
                {
                    ModelState.AddModelError("Image", "Invalid image file.");
                    return View(camera);
                }
            }
            else
            {
                // Jika tidak ada gambar baru yang di-upload, gunakan gambar lama
                camera.Image = existingCamera.Image;
            }

            // Update data kamera
            await _cameraService.UpdateCameraAsync(camera);
            TempData["Message"] = "Camera updated successfully!";
            return RedirectToAction("Index");
        }
        catch (Exception ex)
        {
            _logger.LogError("Error occurred while updating camera: " + ex.Message);
            ModelState.AddModelError("", "An error occurred while updating the camera: " + ex.Message);
            }
        }

        return View(camera);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int cameraId)
    {
        _logger.LogInformation($"Delete action for camera ID: {cameraId}");

        bool isDeleted = await _cameraService.DeleteCameraAsync(cameraId);
        if (!isDeleted)
        {
            _logger.LogWarning($"Delete failed: Camera ID {cameraId} not found.");
            TempData["Error"] = "Camera not found!";
        }
        else
        {
            _logger.LogInformation($"Camera ID {cameraId} deleted successfully.");
            TempData["Message"] = "Camera deleted successfully!";
        }

        return RedirectToAction("Index");
    }
}
