using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CameraRentalApp.Data;
using CameraRentalApp.Models;
using CameraRentalApp.Services;
using System.Threading.Tasks;


public class CameraController : Controller
{
    private readonly CameraService _cameraService;

    public CameraController(ApplicationDbContext context, CameraService cameraService)
    {
        _cameraService = cameraService;
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
            .OrderBy(c => c.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        ViewBag.PageNumber = pageNumber;
        ViewBag.PageSize = pageSize;
        ViewBag.TotalItems = totalItems;
        ViewBag.SearchTerm = searchTerm;

        return View(paginatedCameras);
    }


    //public async Task<IActionResult> Index()
    //{
    //  var cameras = await _cameraService.GetAllCameraAsync();
    // return View(cameras);
    //}

    // public IActionResult Create()
    //{
    //  return View();
    //}
    /*
        [HttpPost]
        public async Task<IActionResult> Create(Camera camera)
        {
            if (ModelState.IsValid)
            {
                await _cameraService.CreateCameraAsync(camera);
                TempData["Message"] = "Success Added Product!";
                return RedirectToAction("Index");
            }
            return View(camera);
        }*/

    [HttpPost]
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
    }


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
    public async Task<IActionResult> Edit(Camera camera)
    {
        if (ModelState.IsValid)
        {
            await _cameraService.UpdateCameraAsync(camera);
            TempData["Message"] = "Success Updated Product!";
            return RedirectToAction("Index");
        }
        return View(camera);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var camera = await _cameraService.GetCameraByIdAsync(id);
        if (camera == null)
        {
            return NotFound();
        }
        return View(camera);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _cameraService.DeleteCameraAsync(id);
        TempData["Message"] = "Camera has been deleted successfully!";
        return RedirectToAction("Index");
    }
}
