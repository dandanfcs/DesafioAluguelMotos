using System.Text;
using Infrastructure.Messaging;
using RabbitMQ.Client;

namespace MyApp.Messaging
{
    public class Producer
    {
        private readonly IModel _channel;

        public Producer(RabbitMqService rabbitMqService)
        {
            _channel = rabbitMqService.Channel;
            _channel.QueueDeclare(queue: "minha_fila", durable: false, exclusive: false, autoDelete: false, arguments: null);
        }

        public void SendMessage(string message)
        {
            var body = Encoding.UTF8.GetBytes(message);
            _channel.BasicPublish(exchange: "", routingKey: "minha_fila", basicProperties: null, body: body);
            Console.WriteLine($"Mensagem enviada: {message}");
        }
    }
}
