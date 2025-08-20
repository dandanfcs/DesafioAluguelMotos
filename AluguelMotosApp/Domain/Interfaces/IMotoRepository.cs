using Domain.Entities;

namespace Domain.Interfaces
{
    public interface IMotoRepository
    {
        Task<Moto> ObterMotoPorIdAsync(string id);
        Task<Moto> ObterMotoPorPlacaAsync(string placa);
        Task<int> AtualizarPlacaDaMotoAsync(string id, string placa);
        Task<List<Moto>> ListarMotosCadastradasAsync();
        Task CadastrarMotoAsync(Moto moto);
        Task RemoverMotoAsync(string id);
    }
}
