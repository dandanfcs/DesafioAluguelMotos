public class PlanoLocacao
{
    public int Dias { get; private set; }
    public decimal PrecoPorDia { get; private set; }

    private PlanoLocacao(int dias, decimal precoPorDia)
    {
        Dias = dias;
        PrecoPorDia = precoPorDia;
    }

    public static readonly PlanoLocacao SeteDias = new(7, 30m);
    public static readonly PlanoLocacao QuinzeDias = new(15, 28m);
    public static readonly PlanoLocacao TrintaDias = new(30, 22m);
    public static readonly PlanoLocacao QuarentaECincoDias = new(45, 20m);
    public static readonly PlanoLocacao CinquentaDias = new(50, 18m);

    public static IEnumerable<PlanoLocacao> Todos => new[]
    {
        SeteDias, QuinzeDias, TrintaDias, QuarentaECincoDias, CinquentaDias
    };

    public static PlanoLocacao ObterPorDias(int dias) => Todos.FirstOrDefault(p => p.Dias == dias)
        ?? throw new InvalidOperationException("Plano inválido.");
}
