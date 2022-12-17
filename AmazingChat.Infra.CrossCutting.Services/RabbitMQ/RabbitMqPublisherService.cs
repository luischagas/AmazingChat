using System.Text;
using AmazingChat.Domain.Interfaces.Services;
using AmazingChat.Domain.Shared.Models;
using Microsoft.Extensions.DependencyInjection;
using RabbitMQ.Client;

namespace AmazingChat.Infra.CrossCutting.Services.RabbitMQ;

 public class RabbitMqPublisherService : IRabbitMqPublisherService
    {
        #region Fields

        private readonly RabbitMqSettings _settings;
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;
        private readonly IModel _channel;

        #endregion Fields

        #region Constructors

        public RabbitMqPublisherService(RabbitMqSettings settings, IServiceScopeFactory serviceScopeFactory)
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

        #endregion ConstructorsInnerException = {OperationInterruptedException} RabbitMQ.Client.Exceptions.OperationInterruptedException: The AMQP operation was interrupted: AMQP close-reason, initiated by Peer, code=530, text='NOT_ALLOWED - vhost Jobsity not found', classId=10, methodId=40\n   at RabbitMQ.Client.Impl.SimpleBlockingRp… View

        #region Methods

        public bool EnqueueMessage(string message)
        {
            var bodyMessage = Encoding.UTF8.GetBytes(message);

            try
            {
                IBasicProperties props = _channel.CreateBasicProperties();
                props.Persistent = true;

                _channel.BasicPublish(
                    exchange: _settings.Queue.Exchange.Name,
                    routingKey: _settings.Queue.RoutingKey,
                    basicProperties: props,
                    body: bodyMessage
                );
            }
            catch (Exception ex)
            {
                return false;
            }

            return true;
        }

        #endregion Methods
    }