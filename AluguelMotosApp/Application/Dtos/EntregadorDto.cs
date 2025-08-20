
using Microsoft.AspNetCore.Http;

namespace Application.Dtos
{
    public class EntregadorDto
    {
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string NumeroCnh { get; set; } = string.Empty;
        public string TipoCnh { get; set; } = string.Empty; // A, B, A+B
        public  IFormFile ImagemCnh { get; set; } 
    }
}
