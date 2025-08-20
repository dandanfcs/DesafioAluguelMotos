namespace Application.Dtos
{
    public class LocacaoDto
    {
        public Guid EntregadorId { get; set; }
        public string MotoId { get; set; }
        public DateTime DataInicio { get; set; }
        public DateTime DataTermino { get; set; }
        public DateTime DataPrevisaoTermino { get; set; }
        public int Plano { get; set; }
    }
}
