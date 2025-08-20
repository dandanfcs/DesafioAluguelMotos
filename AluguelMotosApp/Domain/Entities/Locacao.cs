

using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Locacao
    {
        public Guid Id { get; set; } 
        public string MotoId { get; set; }

        [JsonIgnore] 
        public Moto Moto { get; set; }
        public Guid EntregadorId { get; set; }
        public Entregador Entregador { get; set; }
        public DateTime DataInicio { get; set; } 
        public DateTime DataPrevisaoTermino { get; set; } 
        public DateTime? DataFim { get; set; }
        public decimal valorTotalLocacao { get; set; }
    }
}
