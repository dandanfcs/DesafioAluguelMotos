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


        public MotoService(IMotoRepository motoRepository, IMessagingService messagingService)
        {
            this.motoRepository = motoRepository;
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

        public async Task<Moto> ObterMotoPorPlacaAsync( string placa)
        {
            return await motoRepository.ObterMotoPorPlacaAsync(placa);
        }
        
        public async Task<Moto> ObterMotoPorIdAsync(string id)
        {
            return await motoRepository.ObterMotoPorIdAsync(id);
        }

        public async Task<bool> AtualizarPlacaAsync(string id, string placa)
        {
            var linhasAfetadas = await motoRepository.AtualizarPlacaDaMotoAsync(id,placa);

            if (linhasAfetadas > 0)
                return true;

            return false;
        }

        public async Task<List<Moto>> ListarMotosCadastradasAsync()
        {
            return await motoRepository.ListarMotosCadastradasAsync();
        }
    }
}
