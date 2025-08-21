using Application.Dtos;
using Domain.Entities;
using Microsoft.AspNetCore.Http;

namespace Application.Interfaces
{
    public interface IEntregadorService
    {
        Task AdicionarEntregadorAsync(EntregadorDto entregadorDto, IFormFile imagemCnh);
        Task<List<Entregador>> ObterTodosAsync();
        bool ValidarEntregador(EntregadorDto entregadorDto);
    }
}
