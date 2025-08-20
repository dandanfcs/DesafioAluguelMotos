using Domain.Entities;

namespace Domain.Interfaces
{
    public interface ILocacaoRepository
    {
        Task<Locacao> ObterPorIdAsync(Guid id);
        Task<bool> ExisteAlgumaLocacaoComEsseEntregadorIdEMotoId(Guid entregadorId, string motoId);
        Task<IEnumerable<Locacao>> ObterTodosAsync();
        Task<IEnumerable<Locacao>> ObterPorEntregadorIdAsync(Guid entregadorId);
        Task AdicionarAsync(Locacao locacao);
        Task<bool> ExisteLocacaoAtivaParaMotoAsync(string motoId);
    }
}
