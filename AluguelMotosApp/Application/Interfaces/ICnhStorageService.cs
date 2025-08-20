using Microsoft.AspNetCore.Http;

namespace Application.Interfaces

{
    public interface ICnhStorageService
    {
        Task<string> SaveCnhAsync(Guid entregadorId, IFormFile file);
    }
}