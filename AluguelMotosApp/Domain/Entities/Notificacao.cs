namespace Domain.Entities
{
    public class Notificacao
    {
        public Guid Id { get; set; }
        public string Placa { get; set; }
        public int Ano { get; set; }
        public string Mensagem { get; set; } = string.Empty;
        public DateTime DataEvento { get; set; } 
    }
}
