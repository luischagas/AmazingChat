using AmazingChat.Domain.Shared.Models;
using RabbitMQ.Client;

namespace AmazingChat.Infra.CrossCutting.Services.RabbitMQ;

public static class RabbitMqInitializer
{
    public static void Initiate(IModel channel, RabbitMqSettings settings)
    {
        Dictionary<string, object>? defaultArguments = null;

        channel.QueueDeclare(
            settings.Queue.Name,
            settings.Queue.IsDurable,
            settings.Queue.IsExclusive,
            settings.Queue.IsAutoDeleted,
            defaultArguments
        );

        channel.ExchangeDeclare(
            settings.Queue.Exchange.Name,
            settings.Queue.Exchange.Type,
            settings.Queue.Exchange.IsDurable,
            settings.Queue.Exchange.IsAutoDeleted,
            settings.Queue.Exchange.Arguments
        );

        channel.QueueBind(
            settings.Queue.Name,
            settings.Queue.Exchange.Name,
            settings.Queue.RoutingKey,
            null
        );
    }
}