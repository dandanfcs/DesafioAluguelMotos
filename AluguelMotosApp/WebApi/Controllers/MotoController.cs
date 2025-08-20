using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Numerics;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Admin")] 
    public class MotoController : ControllerBase
    {
        private readonly IMotoService _motoService;
        private readonly ILogger<MotoController> _logger;

        public MotoController(IMotoService motoService, ILogger<MotoController> logger)
        {
            _motoService = motoService;
            _logger = logger;
        }

        /// <summary>
        /// Retorna todas as motos
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ListarMotos()
        {
            _logger.LogInformation("Requisi��o recebida para listar todas as motos");

            var motos = await _motoService.ListarMotosCadastradasAsync();

            _logger.LogInformation("Quantidade de motos encontradas: "+ motos.Count());

            return Ok(motos);
        }

        /// <summary>
        /// Cadastrar uma nova moto
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CadastrarMoto([FromBody] MotoDto motoDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _motoService.CadastrarMotoAsync(motoDto);
            return Ok(new { Message = "Moto adicionada com sucesso!" });
        }

        /// <summary>
        /// Retorna uma moto pelo ID
        /// </summary>
        [HttpGet("{id}")]
        public async Task<IActionResult> ObterMotoPorId(string id)
        {
            var moto = await _motoService.ObterMotoPorIdAsync(id);
            if (moto == null)
                return NotFound(new { Message = "Moto n�o encontrada" });

            return Ok(moto);
        }

  

        /// <summary>
        /// Modificar a placa de uma moto
        /// </summary>
        [HttpPut("{id}/placa")]
        public async Task<IActionResult> AtualizarPlaca([FromRoute] string id, [FromBody] PlacaDto placaDto)
        {
            _logger.LogInformation("Requisi��o recebida para atualizar a placa de Id: {id}", id);

            if (string.IsNullOrWhiteSpace(placaDto.Placa))
                return BadRequest(new { Status = 400, Mensagem = "A placa n�o pode ser vazia." });

            var sucesso = await _motoService.AtualizarPlacaAsync(id, placaDto.Placa);

            if (!sucesso)
                return NotFound(new { Status = 404, Mensagem = $"Moto com id {id} n�o encontrada." });

            return NoContent(); 
        }


        /// <summary>
        /// Consultar Motos existentes
        /// </summary>
        [HttpGet("/placa/{placa}")]
        public async Task<IActionResult> ObterMotoPelaPlaca([FromRoute] string placa)
        {
            _logger.LogInformation("Requisi��o recebida para buscas a placa: {placa}", placa);

            if (string.IsNullOrWhiteSpace(placa))
                return BadRequest(new { Status = 400, Mensagem = "A placa n�o pode ser vazia." });

            var moto = await _motoService.ObterMotoPorPlacaAsync(placa);

            if (moto is null)
                return NotFound(new { Status = 404, Mensagem = $"Moto com placa {placa} n�o encontrada." });

            return Ok(moto);
        }

        /// <summary>
        /// Consultar Motos existentes
        /// </summary>
        [HttpDelete("/placa/{id}")]
        public async Task RemoverMoto([FromRoute] string id)
        {
            _logger.LogInformation("Requisi��o recebida remover a moto de id: {id}", id);

            await _motoService.RemoverMotoAsync(id);
        }
    }
}
