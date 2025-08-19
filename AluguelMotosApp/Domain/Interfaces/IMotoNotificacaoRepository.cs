using Domain.Events;

namespace Domain.Interfaces
{
    public interface IMotoNotificacaoRepository
    {
        Task SalvarAsync(MotoCadastradaEvent evento);
    }
}
