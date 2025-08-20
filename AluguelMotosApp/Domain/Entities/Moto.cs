using System.Text.Json.Serialization;

namespace Domain.Entities
{
    public class Moto
    {
       public string Identificador { get; set; }
        public string Modelo { get; set; }
        public int Ano { get; set; }
        public string Placa { get; set; }
        [JsonIgnore]
        public ICollection<Locacao> Locacoes { get; set; } = new List<Locacao>();
    }
}
