using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RwaMovies.Exceptions;
using RwaMovies.Models.DAL;

namespace RwaMovies.Services
{
    public interface IImagesService
    {
        Task<FileContentResult> GetImage(int id);
        Task<int> PostImage(IFormFile formFile);
        Task PutImage(int id, IFormFile formFile);
        Task DeleteImage(int id);

        public const int MaximumFileSize = 25 * 1000 * 1000; // 25 MB
    }

    public class ImagesService : IImagesService
    {
        private readonly RwaMoviesContext _context;

        public ImagesService(RwaMoviesContext context)
        {
            _context = context;
        }

        public async Task<FileContentResult> GetImage(int id)
        {
            var image = await _context.Images.FindAsync(id);
            if (image == null)
                throw new NotFoundException();
            var imageData = Convert.FromBase64String(image.Content);
            return new FileContentResult(imageData, "image/png");
        }

        public async Task<int> PostImage(IFormFile formFile)
        {
            var imageArray = GetFileByteAray(formFile);
            if (imageArray == null)
                throw new BadRequestException();
            var image = new Image
            {
                Content = Convert.ToBase64String(imageArray)
            };
            _context.Images.Add(image);
            await _context.SaveChangesAsync();
            return image.Id;
        }
        public async Task PutImage(int id, IFormFile formFile)
        {
            try
            {
                var imageArray = GetFileByteAray(formFile);
                if (imageArray == null)
                    throw new BadRequestException();
                var image = new Image
                {
                    Id = id,
                    Content = Convert.ToBase64String(imageArray)
                };
                _context.Entry(image).State = EntityState.Modified;
                await _context.SaveChangesAsync();
            }
            catch
            {
                if (!ImageExists(id))
                    throw new NotFoundException();
                throw;
            }
        }

        public async Task DeleteImage(int id)
        {
            var image = await _context.Images.Include(i => i.Videos).FirstOrDefaultAsync(i => i.Id == id);
            if (image == null)
                throw new NotFoundException();
            _context.Images.Remove(image);
            await _context.SaveChangesAsync();
        }

        private static byte[]? GetFileByteAray(IFormFile formFile)
        {
            if (formFile == null || formFile.Length == 0)
                return null;
            using (var memoryStream = new MemoryStream())
            {
                formFile.CopyTo(memoryStream);
                if (memoryStream.Length < IImagesService.MaximumFileSize)
                    return memoryStream.ToArray();
            }
            return null;
        }
        private bool ImageExists(int id)
        {
            return _context.Images.Any(e => e.Id == id);
        }
    }
}
