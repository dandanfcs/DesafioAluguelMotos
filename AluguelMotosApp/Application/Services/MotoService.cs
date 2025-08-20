using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;
using Microsoft.Extensions.Logging;

namespace Application.Services
{
    public class MotoService : IMotoService
    {
        private IMotoRepository motoRepository;
        private ILocacaoRepository locacaoRepository;
        private readonly ILogger<LocacaoService> _logger;

        public MotoService(IMotoRepository motoRepository, ILocacaoRepository locacaoRepository)
        {
            this.motoRepository = motoRepository;
            this.locacaoRepository = locacaoRepository;
        }

        public async Task CadastrarMotoAsync(MotoDto motoDto)
        {
            _logger.LogInformation("Iniciando cadastro da moto {Identificador} - Placa {Placa}", motoDto.Identificador, motoDto.Placa);

            var moto = new Moto()
            {
                Identificador = motoDto.Identificador,
                Ano = motoDto.Ano,
                Placa = motoDto.Placa,
                Modelo = motoDto.Modelo,
            };

            await motoRepository.CadastrarMotoAsync(moto);
        }

        public async Task<Moto> ObterMotoPorPlacaAsync(string placa)
        {
            _logger.LogInformation("Buscando moto pela Placa {Placa}", placa);

            return await motoRepository.ObterMotoPorPlacaAsync(placa);
        }

        public async Task<Moto> ObterMotoPorIdAsync(string id)
        {
            _logger.LogInformation("Buscando moto pela Id {Placa}", id);
            return await motoRepository.ObterMotoPorIdAsync(id);
        }

        public async Task<bool> AtualizarPlacaAsync(string id, string placa)
        {
            var linhasAfetadas = await motoRepository.AtualizarPlacaDaMotoAsync(id, placa);

            if (linhasAfetadas > 0)
                return true;

            return false;
        }

        public async Task<List<Moto>> ListarMotosCadastradasAsync()
        {
            _logger.LogDebug("Listando todas as motos cadastradas");

            var motos = await motoRepository.ListarMotosCadastradasAsync();

            _logger.LogInformation("{Quantidade} motos encontradas", motos.Count);

            return motos;
        }

        public async Task RemoverMotoAsync(string id)
        {

            _logger.LogInformation("Tentando remover moto {Id}", id);

            var existeLocacao = await locacaoRepository.ExisteLocacaoAtivaParaMotoAsync(id);

            if (existeLocacao)
            {
                _logger.LogError("Não é possível remover a moto {Id}. Existe uma locação ativa vinculada.", id);
                throw new InvalidOperationException("A moto não pode ser removida, pois existe locação ativa para a mesma!");
            }

            await motoRepository.RemoverMotoAsync(id);

            _logger.LogInformation("Moto {Id} removida com sucesso", id);
        }
    }
}
