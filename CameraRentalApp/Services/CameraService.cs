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

        public CameraService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<List<Camera>> GetAllCameraAsync()
        {
            return await _context.Cameras.ToListAsync();
        }

        public async Task<Camera> GetCameraByIdAsync(int id)
        {
            return await _context.Cameras.FindAsync(id);
        }

        
        public async Task<string> UploadImageAsync(IFormFile imageFile)
        {
            var imagesPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/images/cameras");

            if (!Directory.Exists(imagesPath))
            {
                Directory.CreateDirectory(imagesPath);
            }

            var fileName = $"{Guid.NewGuid()}_{Path.GetFileName(imageFile.FileName)}";
            var filePath = Path.Combine(imagesPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(stream);
            }

            return $"/images/cameras/{fileName}";
        }


        public async Task CreateCameraAsync(Camera camera)
        {
            _context.Cameras.Add(camera);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateCameraAsync(Camera camera)
        {
            _context.Cameras.Update(camera);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteCameraAsync(int id)
        {
            var camera = await _context.Cameras.FindAsync(id);
            if (camera != null)
            {
                camera.IsDeleted = true;
                _context.Cameras.Update(camera);
                await _context.SaveChangesAsync();
            }
        }

    }
}
