using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILocacaoService
    {
        Task<Locacao> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Locacao>> ObterTodosAsync();
        Task<IEnumerable<Locacao>> ObterPorEntregadorIdAsync(Guid entregadorId);
        Task AdicionarLocacaoAsync(Locacao locacao);
        Task AtualizarLocacaoAsync(Locacao locacao);
        Task RemoverLocacaoAsync(Guid id);
    }
}
