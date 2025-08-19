using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces
{
    public interface ICnhStorageService
    {
        Task<string> SaveCnhAsync(Guid entregadorId, IFormFile file);
    }
}