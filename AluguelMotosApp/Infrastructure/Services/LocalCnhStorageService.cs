using Application.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Infrastructure.Interfaces
{
    public class LocalCnhStorageService : ICnhStorageService
    {
        private readonly string _storagePath;

        public LocalCnhStorageService()
        {
            _storagePath = Path.Combine(Directory.GetCurrentDirectory(), "CnhStorage");
            if (!Directory.Exists(_storagePath))
                Directory.CreateDirectory(_storagePath);
        }

        public async Task<string> SaveCnhAsync(Guid entregadorId, IFormFile arquivo)
        {
            if (arquivo == null || arquivo.Length == 0)
                throw new ArgumentException("Arquivo inválido.");

            // Cria o nome do arquivo baseado apenas no entregadorId
            var NomeDoArquivo = $"{entregadorId}{Path.GetExtension(arquivo.FileName)}";
            var NomeDaPasta = Path.Combine(_storagePath, NomeDoArquivo);

            // Se já existir um arquivo com o mesmo nome, remove
            if (File.Exists(NomeDaPasta))
            {
                File.Delete(NomeDaPasta);
            }

            // Salva o novo arquivo
            await using var stream = new FileStream(NomeDaPasta, FileMode.Create);
            await arquivo.CopyToAsync(stream);

            return NomeDaPasta;
        }
    }
}
