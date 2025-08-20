using Domain.Entities;
using Domain.Events;
using Domain.Interfaces;
using Infrastructure.Data;

public class MotoNotificacaoRepository : IMotoNotificacaoRepository
{
    private readonly AppDbContext _context;

    public MotoNotificacaoRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task SalvarAsync(MotoCadastradaEvent evento)
    {
        var notificacao = new Notificacao()
        {
            Id = Guid.NewGuid(),
            Ano = evento.Ano,
            Placa = evento.Placa,
            Mensagem = "Moto cadastrada",
            DataEvento = DateTime.Now
        };

        await _context.Notificacoes.AddAsync(notificacao);
        await _context.SaveChangesAsync();
    }
}
