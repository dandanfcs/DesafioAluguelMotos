using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.ComponentModel;
using System.Reflection;

namespace Application.Services
{
    public class EntregadorService : IEntregadorService
    {
        private readonly IEntregadorRepository _repository;
        private readonly ICnhStorageService _cnhStorageService;
        private readonly ILogger<EntregadorService> _logger;

        private enum TiposDeCnhValidos
        {
            [Description("A")]
            A = 1,
            [Description("B")]
            B = 2,
            [Description("A+B")]
            AmaisB = 3
        }

        public EntregadorService(
            IEntregadorRepository repository,
            ICnhStorageService cnhStorageService,
            ILogger<EntregadorService> logger)
        {
            _repository = repository;
            _cnhStorageService = cnhStorageService;
            _logger = logger;
        }

        public async Task AdicionarEntregadorAsync(EntregadorDto entregadorDto, IFormFile imagemCnh)
        {
            _logger.LogInformation("Iniciando cadastro de entregador. Nome={Nome}, CNPJ={Cnpj}", entregadorDto.Nome, entregadorDto.Cnpj);

            try
            {
                await ValidarSeJaExisteEntregadorComOMesmoCnpjOuCnh(entregadorDto);

                ValidarTipoDaCnh(entregadorDto.TipoCnh);

                var entregador = new Entregador()
                {
                    Id = Guid.NewGuid(),
                    Nome = entregadorDto.Nome,
                    Cnpj = entregadorDto.Cnpj,
                    DataNascimento = entregadorDto.DataNascimento,
                    NumeroCnh = entregadorDto.NumeroCnh,
                    TipoCnh = entregadorDto.TipoCnh,
                };

                await _repository.AdicionarAsync(entregador);

                _logger.LogInformation("Entregador cadastrado com sucesso. EntregadorId={EntregadorId}", entregador.Id);

                await _cnhStorageService.SaveCnhAsync(entregador.Id, imagemCnh);

                _logger.LogInformation("CNH salva com sucesso para entregador. EntregadorId={EntregadorId}", entregador.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar entregador. Nome={Nome}, CNPJ={Cnpj}", entregadorDto.Nome, entregadorDto.Cnpj);
                throw;
            }
        }

        private async Task ValidarSeJaExisteEntregadorComOMesmoCnpjOuCnh(EntregadorDto entregadorDto)
        {
            var existente = await _repository.ObterPorCnhOuCnpjAsync(entregadorDto.Cnpj, entregadorDto.NumeroCnh);

            if (existente != null)
            {
                _logger.LogWarning("Falha na validação: já existe entregador com CNPJ={Cnpj} ou CNH={NumeroCnh}",
                    entregadorDto.Cnpj, entregadorDto.NumeroCnh);
                throw new InvalidOperationException("Já existe um entregador cadastrado com este CNPJ ou CNH.");
            }
        }

        public async Task<Entregador> ObterPorIdAsync(Guid id)
        {
            _logger.LogInformation("Buscando entregador por Id={EntregadorId}", id);

            var entregador = await _repository.ObterPorIdAsync(id);
            if (entregador == null)
            {
                _logger.LogWarning("Entregador não encontrado. EntregadorId={EntregadorId}", id);
                throw new KeyNotFoundException("Entregador não encontrado.");
            }

            _logger.LogInformation("Entregador encontrado. EntregadorId={EntregadorId}, Nome={Nome}", entregador.Id, entregador.Nome);
            return entregador;
        }

        public async Task<IEnumerable<Entregador>> ObterTodosAsync()
        {
            _logger.LogInformation("Buscando todos os entregadores.");
            var entregadores = await _repository.ObterTodosAsync();
            _logger.LogInformation("Total de entregadores encontrados: {Quantidade}", entregadores.Count());
            return entregadores;
        }

        public async Task RemoverEntregadorAsync(Guid id)
        {
            _logger.LogInformation("Iniciando remoção de entregador. EntregadorId={EntregadorId}", id);

            var entregador = await _repository.ObterPorIdAsync(id);
            if (entregador == null)
            {
                _logger.LogWarning("Falha ao remover: entregador não encontrado. EntregadorId={EntregadorId}", id);
                throw new KeyNotFoundException("Entregador não encontrado.");
            }

            await _repository.RemoverAsync(id);

            _logger.LogInformation("Entregador removido com sucesso. EntregadorId={EntregadorId}, Nome={Nome}", id, entregador.Nome);
        }

        public bool ValidarEntregador(EntregadorDto entregadorDto)
        {
            var invalido = entregadorDto == null ||
                           string.IsNullOrWhiteSpace(entregadorDto.Nome) ||
                           string.IsNullOrWhiteSpace(entregadorDto.NumeroCnh) ||
                           string.IsNullOrWhiteSpace(entregadorDto.Cnpj);

            if (invalido)
            {
                _logger.LogWarning("Entregador inválido. Nome={Nome}, CNPJ={Cnpj}, CNH={NumeroCnh}",
                    entregadorDto?.Nome, entregadorDto?.Cnpj, entregadorDto?.NumeroCnh);
            }

            return invalido;
        }

        public static void ValidarTipoDaCnh(string tipoCnh)
        {
            if (string.IsNullOrWhiteSpace(tipoCnh))
                throw new ArgumentException("O tipo de CNH não pode ser vazio.");

            tipoCnh = tipoCnh.Trim().ToUpper();

            var tiposValidos = Enum.GetValues(typeof(TiposDeCnhValidos))
                                   .Cast<TiposDeCnhValidos>()
                                   .Select(v => ObterDescricaoDoEnum(v))
                                   .ToList();

            if (!tiposValidos.Contains(tipoCnh))
                throw new InvalidOperationException(
                    $"Tipo de CNH inválido. Tipos permitidos: {string.Join(", ", tiposValidos)}");
        }

        private static string ObterDescricaoDoEnum(Enum valor)
        {
            var campo = valor.GetType().GetField(valor.ToString());
            var attribute = (DescriptionAttribute)campo.GetCustomAttribute(typeof(DescriptionAttribute), false);
            return attribute == null ? valor.ToString() : attribute.Description;
        }
    }
}
