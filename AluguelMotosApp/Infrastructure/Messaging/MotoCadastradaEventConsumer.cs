using Domain.Events;
using Domain.Interfaces;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;

public class MotoCadastradaEventConsumer
{
    private readonly IModel _channel;
    private readonly IMotoNotificacaoRepository _repo;

    public MotoCadastradaEventConsumer(IConnection connection, IMotoNotificacaoRepository repo)
    {
        _channel = connection.CreateModel();
        _repo = repo;
    }

    public void Start()
    {
        _channel.QueueDeclare("motos.cadastradas", false, false, false);
        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += async (model, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            var evento = JsonConvert.DeserializeObject<MotoCadastradaEvent>(json);

            if (evento?.Ano == 2024)
            {
                await _repo.SalvarAsync(evento);
            }
        };

        _channel.BasicConsume("motos.cadastradas", true, consumer);
    }
}
