using Microsoft.AspNetCore.Mvc;
using Application.Interfaces;
using Application.Dtos;
using Microsoft.AspNetCore.Authorization;

namespace WebApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize(Roles = "Entregador")]
    public class EntregadorController : ControllerBase
    {
        private readonly IEntregadorService _entregadorService;
        private readonly ICnhStorageService _cnhStorageService;
        private readonly ILogger<EntregadorController> _logger;

        public EntregadorController(
            IEntregadorService entregadorService,
            ICnhStorageService cnhStorageService,
            ILogger<EntregadorController> logger)
        {
            _entregadorService = entregadorService;
            _cnhStorageService = cnhStorageService;
            _logger = logger;
        }

        /// <summary>
        /// Faz o upload da CNH do entregador
        /// </summary>
        [HttpPost("{entregadorId}/cnh")]
        public async Task<IActionResult> UploadCnh([FromRoute] Guid entregadorId, IFormFile imagemCnh)
        {
            _logger.LogInformation("Iniciando upload da CNH para entregador {EntregadorId}", entregadorId);

            try
            {
                ValidarSeExtensaoDoArquivoEhPermitida(imagemCnh);

                var filePath = await _cnhStorageService.SaveCnhAsync(entregadorId, imagemCnh);

                _logger.LogInformation("Upload da CNH concluído com sucesso. Caminho: {FilePath}", filePath);

                return Ok(new { FilePath = filePath });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fazer upload da CNH para entregador {EntregadorId}", entregadorId);
                return StatusCode(500, "Erro interno ao processar upload da CNH.");
            }
        }

        /// <summary>
        /// Cadastra um novo entregador e sua CNH
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> CadastrarEntregador([FromForm] EntregadorDto entregadorDto)
        {
            _logger.LogInformation("Iniciando cadastro do entregador {Nome}, CNPJ {Cnpj}", entregadorDto.Nome, entregadorDto.Cnpj);

            try
            {
                if (_entregadorService.ValidarEntregador(entregadorDto))
                {
                    _logger.LogWarning("Falha na validação do entregador {Nome}, CNPJ {Cnpj}", entregadorDto.Nome, entregadorDto.Cnpj);
                    return BadRequest(new { Status = 400, Mensagem = "Nome, Email e CPF são obrigatórios." });
                }

                ValidarSeExtensaoDoArquivoEhPermitida(entregadorDto.ImagemCnh);

                await _entregadorService.AdicionarEntregadorAsync(entregadorDto, entregadorDto.ImagemCnh);

                _logger.LogInformation("Entregador {Nome} cadastrado com sucesso!", entregadorDto.Nome);

                return Ok("Entregador cadastrado com sucesso!");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao cadastrar entregador {Nome}, Cnpj {Cnpj}", entregadorDto.Nome, entregadorDto.Cnpj);
                return StatusCode(500, "Erro interno ao cadastrar entregador.");
            }
        }

        /// <summary>
        /// Retorna todos os entregadores
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> ObterEntregadores()
        {
            _logger.LogInformation("Requisição recebida para listar todos os entregadores.");

            try
            {
                var entregadores = await _entregadorService.ObterTodosAsync();
                return Ok(entregadores);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao listar entregadores.");
                return StatusCode(500, "Erro interno ao listar entregadores.");
            }
        }

        private void ValidarSeExtensaoDoArquivoEhPermitida(IFormFile imagemCnh)
        {
            if (imagemCnh == null)
            {
                _logger.LogWarning("Nenhum arquivo de CNH enviado.");
                throw new Exception("Nenhum arquivo enviado.");
            }

            var extensoesPermitidas = new[] { ".png", ".bmp" };
            var extension = Path.GetExtension(imagemCnh.FileName).ToLower();

            if (!extensoesPermitidas.Contains(extension))
            {
                _logger.LogWarning("Arquivo com extensão inválida enviado: {Extensao}", extension);
                throw new Exception("Formato de arquivo inválido. Apenas PNG ou BMP são permitidos.");
            }

            _logger.LogInformation("Arquivo {NomeArquivo} validado com sucesso.", imagemCnh.FileName);
        }
    }
}
