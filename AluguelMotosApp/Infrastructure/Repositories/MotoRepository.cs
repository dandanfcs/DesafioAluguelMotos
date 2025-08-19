using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly AppDbContext _context;

        public MotoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Moto> GetByIdAsync(int id) =>
            await _context.Moto.FindAsync(id);

        public async Task<List<Moto>> GetAllAsync() =>
            await _context.Moto.ToListAsync();

        public async Task AddAsync(Moto aluno)
        {
            await _context.Moto.AddAsync(aluno);
            await _context.SaveChangesAsync();
        }
      
       
    }
}
