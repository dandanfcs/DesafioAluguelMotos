using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Infrastructure.Services;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;
using Domain.Entities;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Entregador")]
    public class EntregadorController : ControllerBase
    {
        private readonly IEntregadorService _entregadorService;
        private readonly ICnhStorageService _cnhStorageService;
        public EntregadorController(IEntregadorService entregadorService, ICnhStorageService cnhStorageService)
        {
            _entregadorService = entregadorService;
            _cnhStorageService = cnhStorageService;
        }

        [HttpPost("{entregadorId}/cnh")]
        public async Task<IActionResult> UploadCnh([FromRoute] Guid entregadorId, IFormFile file)
        {
            if (file == null)
                return BadRequest("Nenhum arquivo enviado.");

            var allowedExtensions = new[] { ".png", ".bmp" };
            var extension = Path.GetExtension(file.FileName).ToLower();

            if (!allowedExtensions.Contains(extension))
                return BadRequest("Formato de arquivo inválido. Apenas PNG ou BMP são permitidos.");

            var filePath = await _cnhStorageService.SaveCnhAsync(entregadorId, file);

            return Ok(new { FilePath = filePath });
        }

        [HttpPost]
        public async Task<IActionResult> CadastrarEntregador([FromBody] EntregadorDto entregadorDto)
        {
            if (_entregadorService.ValidarEntregador(entregadorDto))
            {
                return BadRequest(new { Status = 400, Mensagem = "Nome, Email e CPF são obrigatórios." });
            }

            await _entregadorService.AdicionarEntregadorAsync(entregadorDto);

            return Ok("Entregador cadastrado com sucesso!");
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> ObterEntregadorPorId(Guid id)
        {
            var entregador = await _entregadorService.ObterPorIdAsync(id);
            return Ok(entregador);
        }

    }
}
