using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILocacaoRepository
    {
        Task<Locacao> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Locacao>> ObterTodosAsync();
        Task<IEnumerable<Locacao>> ObterPorEntregadorIdAsync(Guid entregadorId);
        Task<IEnumerable<Locacao>> ObterPorMotoIdAsync(string motoId);
        Task AdicionarAsync(Locacao locacao);
        Task AtualizarAsync(Locacao locacao);
        Task RemoverAsync(Guid id);
        Task<bool> ExisteLocacaoAtivaParaMotoAsync(string motoId);
    }
}
