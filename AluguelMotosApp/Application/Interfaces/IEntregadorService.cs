using Application.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IEntregadorService
    {
        Task AdicionarEntregadorAsync(EntregadorDto entregadorDto, IFormFile imagemCnh);
        Task<IEnumerable<Entregador>> ObterTodosAsync();
        bool ValidarEntregador(EntregadorDto entregadorDto);
    }
}
