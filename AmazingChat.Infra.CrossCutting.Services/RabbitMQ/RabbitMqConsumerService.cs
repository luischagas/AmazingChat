using AmazingChat.Domain.Interfaces.Services;
using AmazingChat.Domain.Shared.Models;
using AmazingChat.Domain.Shared.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AmazingChat.Infra.CrossCutting.Services.RabbitMQ;

 public class RabbitMqConsumerService : IRabbitMqConsumerService
    {
        #region Fields

        private readonly RabbitMqSettings _settings;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        #endregion Fields

        #region Constructors

        public RabbitMqConsumerService(RabbitMqSettings settings)
        {
            _settings = settings;

            _factory = new ConnectionFactory()
            {
                HostName = settings.HostName,
                Port = settings.Port,
                UserName = settings.UserName,
                Password = settings.Password,
            };

            try
            {
                _connection = _factory.CreateConnection();

                _channel = _connection.CreateModel();

                RabbitMqInitializer.Initiate(_channel, settings);
            }
            catch (Exception ex)
            {
               
            }
        }

        #endregion Constructors

        public EventingBasicConsumer DefineBasicConsumer()
        {
            return new EventingBasicConsumer(_channel);
        }

        public IModel GetChannel()
        {
            return _channel;
        }

        public void SetConsumer(EventingBasicConsumer consumer)
        {
            _channel.BasicQos(0, 1, false);

            _channel.BasicConsume(
                queue: _settings.Queue.Name,
                autoAck: false,
                consumer: consumer
            );
        }
    }