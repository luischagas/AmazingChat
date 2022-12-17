using AmazingChat.Domain.Shared.Models;
using RabbitMQ.Client;

namespace AmazingChat.Infra.CrossCutting.Services.RabbitMQ;

 public static class RabbitMqInitializer
    {
        #region Methods

        public static void Initiate(IModel channel, RabbitMqSettings settings)
        {
            Dictionary<string, object> defaultArguments = null;
            
            channel.QueueDeclare(
                queue: settings.Queue.Name,
                durable: settings.Queue.IsDurable,
                exclusive: settings.Queue.IsExclusive,
                autoDelete: settings.Queue.IsAutoDeleted,
                arguments: defaultArguments
            );

            channel.ExchangeDeclare(
                exchange: settings.Queue.Exchange.Name,
                type: settings.Queue.Exchange.Type,
                durable: settings.Queue.Exchange.IsDurable,
                autoDelete: settings.Queue.Exchange.IsAutoDeleted,
                arguments: settings.Queue.Exchange.Arguments
            );

            channel.QueueBind(
                queue: settings.Queue.Name,
                exchange: settings.Queue.Exchange.Name,
                routingKey: settings.Queue.RoutingKey,
                arguments: null
            );
        }

        #endregion Methods
    }