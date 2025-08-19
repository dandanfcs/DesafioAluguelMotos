using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces
{
    public class LocalCnhStorageService : ICnhStorageService
    {
        private readonly string _storagePath;

        public LocalCnhStorageService()
        {
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "CnhStorage");
            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
        }

        public async Task<string> SaveCnhAsync(Guid entregadorId, IFormFile file)
        {
            var fileName = $"{entregadorId}_{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(_storagePath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await file.CopyToAsync(stream);

            return filePath;
        }
    }
}
