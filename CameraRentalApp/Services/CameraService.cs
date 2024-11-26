using CameraRentalApp.Data;
using CameraRentalApp.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CameraRentalApp.Services
{
    public class CameraService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<CameraService> _logger;
        private readonly string _imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "images", "cameras");


        public CameraService(ApplicationDbContext context, ILogger<CameraService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<List<Camera>> GetAllCameraAsync()
        {
            return await _context.Cameras
                                .Where(c => !c.IsDeleted)
                                .OrderByDescending(c => c.CreatedAt)
                                .ToListAsync();
        }

        public async Task<Camera> GetCameraByIdAsync(int cameraId)
        {
            return await _context.Cameras.FindAsync(cameraId);
        }


        public async Task CreateCameraAsync(Camera camera)
        {
            _context.Cameras.Add(camera);
            await _context.SaveChangesAsync();
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            _logger.LogInformation("Uploading image file: {FileName}", file.FileName);

            var fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(_imagePath, fileName);

            if (!Directory.Exists(_imagePath))
            {
                Directory.CreateDirectory(_imagePath);
                _logger.LogInformation("Created directory for images: {ImagePath}", _imagePath);

            }
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            _logger.LogInformation("Image uploaded successfully: {FilePath}", filePath);
            return fileName;
        }

        public async Task DeleteImageAsync(string imagePath)
        {
            var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));

            var fullPathNormalized = Path.GetFullPath(fullPath);
            if (!fullPathNormalized.StartsWith(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot")))
            {
                _logger.LogWarning("Unauthorized file access attempt: {Path}", fullPathNormalized);
                return;
            }

            if (File.Exists(fullPathNormalized))
            {
                try
                {
                    File.Delete(fullPathNormalized);
                    _logger.LogInformation("Image deleted successfully.");
                }
                catch (Exception ex)
                {
                    _logger.LogError("Error deleting image: {Message}", ex.Message);
                }
            }
            else
            {
                _logger.LogWarning("Image not found at path: {ImagePath}", fullPathNormalized);
            }

            await Task.CompletedTask;
        }


        /*        public async Task DeleteImageAsync(string imagePath)
                {
                    var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", imagePath.TrimStart('/'));
                    if (File.Exists(fullPath))
                    {
                        _logger.LogInformation("Deleting image at path: {ImagePath}", fullPath);
                        File.Delete(fullPath);
                        _logger.LogInformation("Image deleted successfully.");

                    }
                    else
                    {
                        _logger.LogWarning("Image not found at path: {ImagePath}", fullPath);
                    }

                    await Task.CompletedTask;
                }*/

        public async Task UpdateCameraAsync(Camera camera)
        {
            try
            {
                var existingCamera = await _context.Cameras.FindAsync(camera.CameraId);
                if (existingCamera == null)
                {
                    _logger.LogWarning("Camera with ID {CameraId} not found.", camera.CameraId);
                    return;
                }

                // Update properties manually
                existingCamera.Name = camera.Name;
                existingCamera.Brand = camera.Brand;
                existingCamera.Stock = camera.Stock;
                existingCamera.RentalPricePerDay = camera.RentalPricePerDay;
                existingCamera.Image = camera.Image; // Jika ada perubahan gambar

                // Set the entity state to modified explicitly
                _context.Entry(existingCamera).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                _logger.LogInformation("Camera with ID {CameraId} updated successfully.", camera.CameraId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating camera with ID: {CameraId}", camera.CameraId);
            }
        }


        /*public async Task UpdateCameraAsync(Camera camera)
        {
            var existingCamera = await _context.Cameras.FindAsync(camera.CameraId);

            if (existingCamera != null)
            {
                existingCamera.Name = camera.Name;
                existingCamera.Brand = camera.Brand;
                existingCamera.Stock = camera.Stock;
                existingCamera.RentalPricePerDay = camera.RentalPricePerDay;

                if (!string.IsNullOrEmpty(camera.Image))
                {
                    existingCamera.Image = camera.Image;
                }

                await _context.SaveChangesAsync();
            }
         }*/


        public async Task<bool> DeleteCameraAsync(int cameraId)
        {
            _logger.LogInformation($"Attempting to delete camera with ID: {cameraId}");

            var camera = await GetCameraByIdAsync(cameraId);

            if (camera == null)
            {
                _logger.LogWarning($"Camera with ID: {cameraId} not found.");
                return false; 
            }
            {
                    camera.IsDeleted = true;
                    _context.Cameras.Update(camera);
                    await _context.SaveChangesAsync();

                    _logger.LogInformation($"Customer with ID: {cameraId} marked as deleted.");
                    return true;
                }
            }
        }
    }