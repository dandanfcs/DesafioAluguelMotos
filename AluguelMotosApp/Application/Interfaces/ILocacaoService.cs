using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface ILocacaoService
    {
        Task<Locacao> ObterPorIdAsync(Guid id);
        Task<IEnumerable<Locacao>> ObterTodasLocacoesAsync();
        Task<IEnumerable<Locacao>> ObterPorEntregadorIdAsync(Guid entregadorId);
        Task<Locacao> CriarLocacaoAsync(LocacaoDto locacaoDto);
        Task<decimal> CalcularValorFinalAsync(Guid locacaoId, DateTime dataDevolucao);
    }
}
