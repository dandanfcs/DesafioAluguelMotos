using Microsoft.AspNetCore.Http;

namespace Infrastructure.Services
{
    public interface ICnhStorageService
    {
        Task<string> SaveCnhAsync(Guid entregadorId, IFormFile file);
    }
}