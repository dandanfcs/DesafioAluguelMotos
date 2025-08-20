using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IEntregadorRepository
    {
        Task<Entregador> ObterPorIdAsync(Guid id);
        Task<Entregador> ObterPorCnhOuCnpjAsync(string cnpj, string numeroCnh);
        Task<IEnumerable<Entregador>> ObterTodosAsync();
        Task AdicionarAsync(Entregador entregador);
        Task AtualizarAsync(Entregador entregador);
        Task RemoverAsync(Guid id);
    }
}