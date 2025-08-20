using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class LocacaoService : ILocacaoService
    {
        private readonly ILocacaoRepository _locacaoRepository;

        public LocacaoService(ILocacaoRepository locacaoRepository)
        {
            _locacaoRepository = locacaoRepository;
        }

        public async Task AdicionarLocacaoAsync(Locacao locacao)
        {
            bool motoAlugada = await _locacaoRepository.ExisteLocacaoAtivaParaMotoAsync(locacao.MotoId);
            if (motoAlugada)
                throw new InvalidOperationException("Esta moto já está alugada no momento.");

            locacao.Id = Guid.NewGuid();
            locacao.DataInicio = DateTime.UtcNow;

            await _locacaoRepository.AdicionarAsync(locacao);
        }

        public async Task AtualizarLocacaoAsync(Locacao locacao)
        {
            var locacaoExistente = await _locacaoRepository.ObterPorIdAsync(locacao.Id);
            if (locacaoExistente == null)
                throw new KeyNotFoundException("Locação não encontrada.");

            if (locacaoExistente.DataFim != null)
                throw new InvalidOperationException("Não é possível atualizar uma locação já encerrada.");

            locacaoExistente.DataFim = locacao.DataFim;
            await _locacaoRepository.AtualizarAsync(locacaoExistente);
        }

        public async Task<Locacao> ObterPorIdAsync(Guid id)
        {
            var locacao = await _locacaoRepository.ObterPorIdAsync(id);
            if (locacao == null)
                throw new KeyNotFoundException("Locação não encontrada.");

            return locacao;
        }

        public async Task<IEnumerable<Locacao>> ObterTodosAsync()
        {
            return await _locacaoRepository.ObterTodosAsync();
        }

        public async Task<IEnumerable<Locacao>> ObterPorEntregadorIdAsync(Guid entregadorId)
        {
            return await _locacaoRepository.ObterPorEntregadorIdAsync(entregadorId);
        }

        public async Task RemoverLocacaoAsync(Guid id)
        {
            var locacao = await _locacaoRepository.ObterPorIdAsync(id);
            if (locacao == null)
                throw new KeyNotFoundException("Locação não encontrada.");

            if (locacao.DataFim == null)
                throw new InvalidOperationException("Não é possível remover uma locação em andamento.");

            await _locacaoRepository.RemoverAsync(id);
        }
    }
}
