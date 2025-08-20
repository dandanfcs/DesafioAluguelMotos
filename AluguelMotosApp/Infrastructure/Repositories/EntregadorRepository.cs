using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Infrastructure.Repositories
{
    public class EntregadorRepository : IEntregadorRepository
    {
        private readonly AppDbContext _context;

        public EntregadorRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Entregador entregador)
        {

            await _context.Entregador.AddAsync(entregador);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Entregador entregador)
        {
            _context.Entregador.Update(entregador);
            await _context.SaveChangesAsync();
        }

        public async Task<Entregador> ObterPorCnhOuCnpjAsync(string cnpj, string numeroCnh)
        {
            return await _context.Entregador
                                 .FirstOrDefaultAsync(e => e.Cnpj == cnpj || e.NumeroCnh == numeroCnh);
        }

        public async Task<Entregador> ObterPorIdAsync(Guid id)
        {
            return await _context.Entregador.FindAsync(id);
        }

        public async Task<IEnumerable<Entregador>> ObterTodosAsync()
        {
            return await _context.Entregador.ToListAsync();
        }

        public async Task RemoverAsync(Guid id)
        {
            var entregador = await _context.Entregador.FindAsync(id);
            if (entregador != null)
            {
                _context.Entregador.Remove(entregador);
                await _context.SaveChangesAsync();
            }
        }
    }
}
