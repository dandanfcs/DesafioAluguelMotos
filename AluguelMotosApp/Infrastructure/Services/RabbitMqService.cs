using Application.Interfaces;
using Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RabbitMQ.Client;
using System.Text;

namespace Infrastructure.Services
{
    public class RabbitMqService : IMessagingService, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMqService> _logger;

        public IModel Channel => _channel;

        public RabbitMqService(IOptions<RabbitMqSettings> options, ILogger<RabbitMqService> logger)
        {
            _logger = logger;
            var settings = options.Value;

            try
            {
                var factory = new ConnectionFactory
                {
                    HostName = settings.HostName,
                    UserName = settings.UserName,
                    Password = settings.Password
                };

                _connection = factory.CreateConnection();
                _channel = _connection.CreateModel();

                _logger.LogInformation("Conexão com RabbitMQ criada com sucesso. Host={HostName}", settings.HostName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao criar conexão com RabbitMQ. Host={HostName}", settings.HostName);
                throw;
            }
        }

        public void Dispose()
        {
            try
            {
                _channel?.Close();
                _connection?.Close();
                _logger.LogInformation("Conexão e canal RabbitMQ fechados com sucesso.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao fechar conexão/canal RabbitMQ.");
            }
        }

        public Task PublishAsync<T>(T message, string topic)
        {
            _logger.LogInformation("Iniciando publicação de mensagem no RabbitMQ. Tópico={Topic}, Tipo={MessageType}", topic, typeof(T).Name);

            try
            {
                _channel.QueueDeclare(queue: topic, durable: false, exclusive: false, autoDelete: false);

                var json = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(json);

                _channel.BasicPublish("", topic, null, body);

                _logger.LogInformation("Mensagem publicada com sucesso. Tópico={Topic}, Tamanho={MessageSize} bytes", topic, body.Length);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erro ao publicar mensagem no RabbitMQ. Tópico={Topic}", topic);
                throw;
            }

            return Task.CompletedTask;
        }
    }
}
