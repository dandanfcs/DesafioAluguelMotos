using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IEntregadorService
    {
        Task<Entregador> ObterPorIdAsync(Guid id);
        Task AdicionarEntregadorAsync(EntregadorDto entregador);
        Task<IEnumerable<Entregador>> ObterTodosAsync();
        bool ValidarEntregador(EntregadorDto entregadorDto);
    }
}
