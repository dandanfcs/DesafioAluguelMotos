using Application.Interfaces;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Text;

namespace Infrastructure.Messaging
{
    public class RabbitMqService : IMessagingService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        public IModel Channel => _channel;

        public RabbitMqService(IOptions<RabbitMqSettings> options)
        {
            var settings = options.Value;

            var factory = new ConnectionFactory
            {
                HostName = settings.HostName,
                UserName = settings.UserName,
                Password = settings.Password
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
        }

        public void Dispose()
        {
            _channel?.Close();
            _connection?.Close();
        }

        public Task PublishAsync<T>(T message, string topic)
        {
            _channel.QueueDeclare(queue: topic, durable: false, exclusive: false, autoDelete: false);

            var json = JsonConvert.SerializeObject(message);
            var body = Encoding.UTF8.GetBytes(json);

            _channel.BasicPublish("", topic, null, body);

            return Task.CompletedTask;
        }
    }
}
