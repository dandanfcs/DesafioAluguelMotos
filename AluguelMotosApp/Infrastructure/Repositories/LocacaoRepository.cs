using Domain.Entities;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;


namespace Infrastructure.Repositories
{
    public class LocacaoRepository : ILocacaoRepository
    {
        private readonly AppDbContext _context;

        public LocacaoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task AdicionarAsync(Locacao locacao)
        {
            await _context.Locacao.AddAsync(locacao);
            await _context.SaveChangesAsync();
        }

        public async Task AtualizarAsync(Locacao locacao)
        {
            _context.Locacao.Update(locacao);
            await _context.SaveChangesAsync();
        }

        public async Task<Locacao> ObterPorIdAsync(Guid id)
        {
            return await _context.Locacao
                                 .Include(l => l.Moto)
                                 .Include(l => l.Entregador)
                                 .FirstOrDefaultAsync(l => l.Id == id);
        }

        public async Task<IEnumerable<Locacao>> ObterTodosAsync()
        {
            return await _context.Locacao
                                 .Include(l => l.Moto)
                                 .Include(l => l.Entregador)
                                 .ToListAsync();
        }

        public async Task<IEnumerable<Locacao>> ObterPorEntregadorIdAsync(Guid entregadorId)
        {
            return await _context.Locacao
                                 .Include(l => l.Moto)
                                 .Where(l => l.EntregadorId == entregadorId)
                                 .ToListAsync();
        }

        public async Task<bool> ExisteLocacaoAtivaParaMotoAsync(string motoId)
        {
            return await _context.Locacao
                                 .AnyAsync(l => l.MotoId == motoId && l.DataFim == null);
        }

        public async Task<bool> ExisteAlgumaLocacaoComEsseEntregadorIdEMotoId(Guid entregadorId, string motoId)
        {
            return await _context.Locacao
                                .Where(l => l.EntregadorId == entregadorId
                                && l.MotoId == motoId)
                                .AnyAsync();
        }
    }
}
