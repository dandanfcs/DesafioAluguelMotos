using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class LocacaoService : ILocacaoService
    {
        private readonly ILocacaoRepository _locacaoRepository;
        private readonly IEntregadorRepository _entregadorRepository;
        private readonly IMotoRepository _motoRepository;
        private readonly ILogger<LocacaoService> _logger;

        public LocacaoService(
            ILocacaoRepository locacaoRepository,
            IEntregadorRepository entregadorRepository,
            IMotoRepository motoRepository,
            ILogger<LocacaoService> logger)
        {
            _locacaoRepository = locacaoRepository;
            _entregadorRepository = entregadorRepository;
            _motoRepository = motoRepository;
            _logger = logger;
        }

        public async Task<Locacao> CriarLocacaoAsync(LocacaoDto locacaoDto)
        {
            _logger.LogInformation("Iniciando criação de locação. EntregadorId={EntregadorId}, MotoId={MotoId}, Plano={Plano}",
                locacaoDto.EntregadorId, locacaoDto.MotoId, locacaoDto.Plano);

            var entregador = await _entregadorRepository.ObterPorIdAsync(locacaoDto.EntregadorId);

            if (entregador is null || entregador.TipoCnh != "A")
            {
                _logger.LogWarning("Falha ao criar locação: entregador inválido ou CNH não é categoria A. EntregadorId={EntregadorId}", locacaoDto.EntregadorId);
                throw new InvalidOperationException("Somente entregadores com CNH categoria A podem alugar motos.");
            }

            var moto = await _motoRepository.ObterMotoPorIdAsync(locacaoDto.MotoId);

            if (moto is null)
            {
                _logger.LogWarning("Falha ao criar locação: moto não encontrada. MotoId={MotoId}", locacaoDto.MotoId);
                throw new InvalidOperationException("Moto não encontrada.");
            }

            var existe = await _locacaoRepository.ExisteAlgumaLocacaoComEsseEntregadorIdEMotoId(locacaoDto.EntregadorId, locacaoDto.MotoId);

            if (existe)
            {
                _logger.LogWarning("Falha ao criar locação: já existe locação ativa. EntregadorId={EntregadorId}, MotoId={MotoId}",
                    locacaoDto.EntregadorId, locacaoDto.MotoId);
                throw new InvalidOperationException("A moto já está alugada para esse entregador");
            }

            Locacao locacao = ObterLocacaoASerInserida(locacaoDto);
            await _locacaoRepository.AdicionarAsync(locacao);

            return locacao;
        }

        private static Locacao ObterLocacaoASerInserida(LocacaoDto locacaoDto)
        {
            var plano = PlanoLocacao.ObterPorDias(locacaoDto.Plano);
            decimal precoPorDia = plano.PrecoPorDia;

            var dataCriacao = DateTime.UtcNow;
            var dataInicio = dataCriacao.Date.AddDays(1);
            var dataPrevisaoTermino = dataInicio.AddDays(locacaoDto.Plano);
            var dataTermino = dataPrevisaoTermino;
            var valorTotal = locacaoDto.Plano * precoPorDia;

            return new Locacao()
            {
                EntregadorId = locacaoDto.EntregadorId,
                MotoId = locacaoDto.MotoId,
                DataInicio = dataInicio,
                DataFim = dataTermino,
                DataPrevisaoTermino = dataPrevisaoTermino,
                valorTotalLocacao = valorTotal
            };
        }

        public async Task<decimal> CalcularValorFinalAsync(Guid locacaoId, DateTime dataDevolucao)
        {
            var locacao = await _locacaoRepository.ObterPorIdAsync(locacaoId);

            if (locacao is null)
            {
                _logger.LogError("Locação não encontrada ao calcular valor final. LocacaoId={LocacaoId}", locacaoId);
                throw new InvalidOperationException("Locação não encontrada.");
            }

            var plano = PlanoLocacao.ObterPorDias((locacao.DataPrevisaoTermino - locacao.DataInicio).Days);

            int diasEfetivados = (dataDevolucao.Date - locacao.DataInicio.Date).Days;
            if (diasEfetivados < 1)
            {
                _logger.LogWarning("Data de devolução inválida. LocacaoId={LocacaoId}, DataDevolucao={DataDevolucao}",
                    locacaoId, dataDevolucao);
                throw new InvalidOperationException("Data de devolução inválida.");
            }

            var valorFinal = CalcularValorFinalDaLocaocao(dataDevolucao, locacao, plano, diasEfetivados);

            return valorFinal;
        }

        private static decimal CalcularValorFinalDaLocaocao(DateTime dataDevolucao, Locacao? locacao, PlanoLocacao plano, int diasEfetivados)
        {
            if (DevolucaoAntecipada(dataDevolucao, locacao))
            {
               return CalcularValorDeDevolucaoAntecipada(plano, diasEfetivados);
            }
            else if (DevolucaoAtrasada(dataDevolucao, locacao))
            {
                return CalcularValorDeDevolucaoAtrasada(dataDevolucao, locacao);
            }

            return locacao.valorTotalLocacao;
        }

        private static decimal CalcularValorDeDevolucaoAtrasada(DateTime dataDevolucao, Locacao? locacao)
        {
            decimal valorFinal;
            int diasAdicionais = (dataDevolucao.Date - locacao.DataPrevisaoTermino.Date).Days;
            var valorAdicional = diasAdicionais * 50m; // R$50 por dia extra

            valorFinal = locacao.valorTotalLocacao + valorAdicional;
            return valorFinal;
        }

        private static decimal CalcularValorDeDevolucaoAntecipada(PlanoLocacao plano, int diasEfetivados)
        {
            decimal valorFinal;
            var valorDiariasUsadas = diasEfetivados * plano.PrecoPorDia;
            int diasNaoEfetivados = plano.Dias - diasEfetivados;
            decimal valorDiariasNaoUsadas = diasNaoEfetivados * plano.PrecoPorDia;

            decimal multa = plano.Dias switch
            {
                7 => valorDiariasNaoUsadas * 0.20m, // 20%
                15 => valorDiariasNaoUsadas * 0.40m, // 40%
                _ => 0 // planos maiores não têm multa (conforme regra)
            };

            valorFinal = valorDiariasUsadas + multa;
            return valorFinal;
        }

        private static bool DevolucaoAntecipada(DateTime dataDevolucao, Locacao? locacao)
        {
            return dataDevolucao.Date < locacao.DataPrevisaoTermino.Date;
        }

        private static bool DevolucaoAtrasada(DateTime dataDevolucao, Locacao locacao)
        {
            return dataDevolucao.Date > locacao.DataPrevisaoTermino.Date;
        }

        public async Task<Locacao> ObterPorIdAsync(Guid id)
        {
            var locacao = await _locacaoRepository.ObterPorIdAsync(id);
            if (locacao is null)
            {
                _logger.LogError("Locação não encontrada ao calcular valor final. LocacaoId={LocacaoId}", id);
                throw new InvalidOperationException("Locação não encontrada.");
            }

            return locacao;
        }

        public async Task<IEnumerable<Locacao>> ObterTodasLocacoesAsync()
        {
            return await _locacaoRepository.ObterTodosAsync();
        }

        public async Task<IEnumerable<Locacao>> ObterPorEntregadorIdAsync(Guid entregadorId)
        {
            return await _locacaoRepository.ObterPorEntregadorIdAsync(entregadorId);
        }
    }
}
