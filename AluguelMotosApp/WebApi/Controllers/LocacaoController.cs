using Application.Dtos;
using Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    [Authorize(Roles = "Entregador")] 
    public class LocacaoController : ControllerBase
    {
        private readonly ILocacaoService _locacaoService;

        public LocacaoController(ILocacaoService locacaoService)
        {
            _locacaoService = locacaoService;
        }

        /// <summary>
        /// Alugar uma moto
        /// </summary>
        [HttpPost]
        public async Task AlugarMoto([FromBody] LocacaoDto locacaoDto)
        {
            await _locacaoService.CriarLocacaoAsync(locacaoDto);
        }

        /// <summary>
        /// Consultar Locação por Id
        /// </summary>
        [HttpGet("/{locacaoId}")]
        public async Task<IActionResult> ObterLocacao([FromRoute] Guid locacaoId)
        {
            var locacao = await _locacaoService.ObterPorIdAsync(locacaoId);

            return Ok(locacao);
        }

        /// <summary>
        /// Informar data de devolução e calcular valor 
        /// </summary>
        [HttpPut("/{locacaoId}/devolucao")]
        public async Task<IActionResult> ConsultarLocacao([FromRoute] Guid locacaoId, [FromBody] DataDevolucaoLocacaoDto dataDevolucaoDto)
        {
            var valorLocacao = await _locacaoService.CalcularValorFinalAsync(locacaoId, dataDevolucaoDto.DataDevolucao);
       
           return Ok("O valor da locacao eh: "+ valorLocacao);
        }
    }
}
