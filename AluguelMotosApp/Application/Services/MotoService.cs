using Application.Dtos;
using Application.Interfaces;
using Domain.Entities;
using Domain.Interfaces;

namespace Application.Services
{
    public class MotoService : IMotoService
    {
        private IMotoRepository motoRepository;

        public MotoService(IMotoRepository motoRepository)
        {
            this.motoRepository = motoRepository;
        }

        public async Task AddAsync(MotoDto motoDto)
        {
            var moto = new Moto()
            {
                Identificador = motoDto.Identificador,
                Ano = motoDto.Ano,
                Placa = motoDto.Placa,
                Modelo = motoDto.Modelo,    
            };

           await motoRepository.AddAsync(moto);
        }

        public async Task<List<Moto>> GetAllAsync()
        {
            return await motoRepository.GetAllAsync();
        }

        public async Task<Moto> GetByIdAsync(int id)
        {
            return await motoRepository.GetByIdAsync(id);
        }
    }
}
