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


        /// <summary>
        /// Faz o upload da CNH do entregador
        /// </summary>
        [HttpPost("{entregadorId}/cnh")]
        public async Task<IActionResult> UploadCnh([FromRoute] Guid entregadorId, IFormFile imagemCnh)
        {
            ValidarSeExtensaoDoArquivoEhPermitida(imagemCnh);

            var filePath = await _cnhStorageService.SaveCnhAsync(entregadorId, imagemCnh);

            return Ok(new { FilePath = filePath });
        }

        /// <summary>
        /// Cadastra um novo entregador e sua CNH
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CadastrarEntregador([FromForm] EntregadorDto entregadorDto)
        {
            if (_entregadorService.ValidarEntregador(entregadorDto))
            {
                return BadRequest(new { Status = 400, Mensagem = "Nome, Email e CPF são obrigatórios." });
            }

            ValidarSeExtensaoDoArquivoEhPermitida(entregadorDto.ImagemCnh);

            await _entregadorService.AdicionarEntregadorAsync(entregadorDto, entregadorDto.ImagemCnh);

            return Ok("Entregador cadastrado com sucesso!");
        }


        /// <summary>
        /// Retorna todos os entregadores
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObterEntregadores()
        {
            var entregador = await _entregadorService.ObterTodosAsync();
            return Ok(entregador);
        }

        private void ValidarSeExtensaoDoArquivoEhPermitida(IFormFile imagemCnh)
        {
            if (imagemCnh == null)
                throw new Exception("Nenhum arquivo enviado.");

            var extensoesPermitidas = new[] { ".png", ".bmp" };
            var extension = Path.GetExtension(imagemCnh.FileName).ToLower();

            if (!extensoesPermitidas.Contains(extension))
                throw new Exception("Formato de arquivo inválido. Apenas PNG ou BMP são permitidos.");
        }

    }
}
