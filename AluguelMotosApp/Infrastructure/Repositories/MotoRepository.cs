using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories
{
    public class MotoRepository : IMotoRepository
    {
        private readonly AppDbContext _context;
        private IMessagingService _messagingService;

        public MotoRepository(AppDbContext context, IMessagingService messagingService)
        {
            _context = context;
            _messagingService = messagingService;
        }

        public async Task<Moto> ObterMotoPorIdAsync(string id) =>
            await _context.Moto.Where(x => x.Identificador == id).FirstOrDefaultAsync();

        public async Task<List<Moto>> ListarMotosCadastradasAsync() =>
            await _context.Moto.ToListAsync();

        public async Task CadastrarMotoAsync(Moto moto)
        {
            await _context.Moto.AddAsync(moto);
            await _context.SaveChangesAsync();

            await PublicarEventoMotoCadastrada(moto);
        }

        private async Task PublicarEventoMotoCadastrada(Moto moto)
        {
            var evento = new MotoCadastradaEvent
            {
                Id = moto.Identificador,
                Placa = moto.Placa,
                Ano = moto.Ano
            };

            await _messagingService.PublishAsync(evento, "motos.cadastradas");
        }

        public async Task<Moto> ObterMotoPorPlacaAsync(string placa)
        {
            return await _context.Moto.Where(x => x.Placa == placa).FirstOrDefaultAsync();
        }
        public async Task<int> AtualizarPlacaDaMotoAsync(string id, string novaPlaca)
        {
           return await _context.Moto.Where(x => x.Identificador == id)
                         .ExecuteUpdateAsync(s => s.SetProperty(m => m.Placa, novaPlaca));
        }

    }
}
