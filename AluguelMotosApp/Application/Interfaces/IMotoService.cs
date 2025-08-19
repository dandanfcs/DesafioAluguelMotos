

using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMotoService
    {
        Task<Moto> GetByIdAsync(int id);
        Task<List<Moto>> GetAllAsync();
        Task AddAsync(MotoDto motoDto);
    }
}
