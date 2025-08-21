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
        private readonly ILogger<LocacaoController> _logger;

        public LocacaoController(ILocacaoService locacaoService, ILogger<LocacaoController> logger)
        {
            _locacaoService = locacaoService;
            _logger = logger;
        }

        /// <summary>
        /// Alugar uma moto
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> AlugarMoto([FromBody] LocacaoDto locacaoDto)
        {
            _logger.LogInformation("Recebida requisição para criar locação. EntregadorId={EntregadorId}, MotoId={MotoId}, Plano={Plano}",
                locacaoDto.EntregadorId, locacaoDto.MotoId, locacaoDto.Plano);

            try
            {
                var locacao = await _locacaoService.CriarLocacaoAsync(locacaoDto);

                _logger.LogInformation("Locação criada com sucesso. LocacaoId={LocacaoId}, EntregadorId={EntregadorId}, MotoId={MotoId}",
                    locacao.Id, locacaoDto.EntregadorId, locacaoDto.MotoId);

                return Ok(locacao);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar locação. EntregadorId={EntregadorId}, MotoId={MotoId}",
                    locacaoDto.EntregadorId, locacaoDto.MotoId);
                return StatusCode(500, "Erro interno ao criar locação.");
            }
        }

        /// <summary>
        /// Consultar Locação por Id
        /// </summary>
        [HttpGet("/{locacaoId}")]
        public async Task<IActionResult> ObterLocacao([FromRoute] Guid locacaoId)
        {
            _logger.LogInformation("Recebida requisição para obter locação. LocacaoId={LocacaoId}", locacaoId);

            try
            {
                var locacao = await _locacaoService.ObterPorIdAsync(locacaoId);

                _logger.LogInformation("Locação obtida com sucesso. LocacaoId={LocacaoId}", locacaoId);

                return Ok(locacao);
            }
            catch (KeyNotFoundException)
            {
                _logger.LogWarning("Locação não encontrada. LocacaoId={LocacaoId}", locacaoId);
                return NotFound("Locação não encontrada.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao obter locação. LocacaoId={LocacaoId}", locacaoId);
                return StatusCode(500, "Erro interno ao consultar locação.");
            }
        }

        /// <summary>
        /// Informar data de devolução e calcular valor 
        /// </summary>
        [HttpPut("/{locacaoId}/devolucao")]
        public async Task<IActionResult> ConsultarLocacao([FromRoute] Guid locacaoId, [FromBody] DataDevolucaoLocacaoDto dataDevolucaoDto)
        {
            _logger.LogInformation("Recebida requisição para calcular valor final da locação. LocacaoId={LocacaoId}, DataDevolucao={DataDevolucao}",
                locacaoId, dataDevolucaoDto.DataDevolucao);

            try
            {
                var valorLocacao = await _locacaoService.CalcularValorFinalAsync(locacaoId, dataDevolucaoDto.DataDevolucao);

                _logger.LogInformation("Cálculo do valor final concluído. LocacaoId={LocacaoId}, ValorFinal={ValorFinal}",
                    locacaoId, valorLocacao);

                return Ok(valorLocacao);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Erro de regra de negócio ao calcular valor da locação. LocacaoId={LocacaoId}", locacaoId);
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao calcular valor da locação. LocacaoId={LocacaoId}", locacaoId);
                return StatusCode(500, "Erro interno ao calcular valor da locação.");
            }
        }
    }
}
