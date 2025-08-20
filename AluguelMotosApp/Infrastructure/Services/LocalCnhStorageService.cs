using Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Infrastructure.Interfaces
{
    public class LocalCnhStorageService : ICnhStorageService
    {
        private readonly string _storagePath;
        private readonly ILogger<LocalCnhStorageService> _logger;

        public LocalCnhStorageService(ILogger<LocalCnhStorageService> logger)
        {
            _logger = logger;
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "CnhStorage");
            if (!Directory.Exists(_storagePath))
            {
                Directory.CreateDirectory(_storagePath);
                _logger.LogInformation("Pasta de armazenamento criada em {StoragePath}", _storagePath);
            }
        }

        public async Task<string> SaveCnhAsync(Guid entregadorId, IFormFile arquivo)
        {
            _logger.LogInformation("Iniciando salvamento de CNH. EntregadorId={EntregadorId}, NomeArquivo={NomeArquivo}",
                entregadorId, arquivo?.FileName);

            if (arquivo == null || arquivo.Length == 0)
            {
                _logger.LogWarning("Arquivo inválido para EntregadorId={EntregadorId}", entregadorId);
                throw new ArgumentException("Arquivo inválido.");
            }

            var nomeDoArquivo = $"{entregadorId}{Path.GetExtension(arquivo.FileName)}";
            var caminhoCompleto = Path.Combine(_storagePath, nomeDoArquivo);

            try
            {
                if (File.Exists(caminhoCompleto))
                {
                    File.Delete(caminhoCompleto);
                    _logger.LogInformation("Arquivo existente deletado: {Caminho}", caminhoCompleto);
                }

                await using var stream = new FileStream(caminhoCompleto, FileMode.Create);
                await arquivo.CopyToAsync(stream);

                _logger.LogInformation("Arquivo salvo com sucesso. EntregadorId={EntregadorId}, Caminho={Caminho}",
                    entregadorId, caminhoCompleto);

                return caminhoCompleto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao salvar CNH. EntregadorId={EntregadorId}, Caminho={Caminho}",
                    entregadorId, caminhoCompleto);
                throw;
            }
        }
    }
}
