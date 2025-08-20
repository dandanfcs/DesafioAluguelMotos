using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;

namespace Application.Services
{
    public class MotoService : IMotoService
    {
        private IMotoRepository motoRepository;
        private ILocacaoRepository locacaoRepository;

        public MotoService(IMotoRepository motoRepository, ILocacaoRepository locacaoRepository)
        {
            this.motoRepository = motoRepository;
            this.locacaoRepository = locacaoRepository;
        }

        public async Task CadastrarMotoAsync(MotoDto motoDto)
        {
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
            return await motoRepository.ObterMotoPorPlacaAsync(placa);
        }

        public async Task<Moto> ObterMotoPorIdAsync(string id)
        {
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
            return await motoRepository.ListarMotosCadastradasAsync();
        }

        public async Task RemoverMotoAsync(string id)
        {
            var existeLocacao = await locacaoRepository.ExisteLocacaoAtivaParaMotoAsync(id);

            if (existeLocacao)
                throw new Exception("A moto nao pode ser removida, pois existe locacao para a mesma!");

            await motoRepository.RemoverMotoAsync(id);
        }
    }
}
