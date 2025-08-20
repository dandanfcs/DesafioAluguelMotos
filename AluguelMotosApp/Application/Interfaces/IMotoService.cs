

using Application.Dtos;
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IMotoService
    {
        Task<Moto> ObterMotoPorIdAsync(string id);
        Task<List<Moto>> ListarMotosCadastradasAsync();
        Task CadastrarMotoAsync(MotoDto motoDto);
        Task<Moto> ObterMotoPorPlacaAsync(string placa);
        Task<bool> AtualizarPlacaAsync(string id, string placa);
        Task RemoverMotoAsync(string id);
    }
}
