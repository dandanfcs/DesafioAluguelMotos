
namespace Domain.Entities
{
    public class Entregador
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Nome { get; set; } = string.Empty;
        public string Cnpj { get; set; } = string.Empty;
        public DateTime DataNascimento { get; set; }
        public string NumeroCnh { get; set; } = string.Empty;
        public string TipoCnh { get; set; } = string.Empty; // A, B, A+B
        public ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}
