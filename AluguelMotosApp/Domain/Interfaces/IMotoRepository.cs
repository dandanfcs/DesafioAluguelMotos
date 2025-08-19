using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMotoRepository
    {
        Task<Moto> GetByIdAsync(int id);
        Task<List<Moto>> GetAllAsync();
        Task AddAsync(Moto moto);
    }
}
