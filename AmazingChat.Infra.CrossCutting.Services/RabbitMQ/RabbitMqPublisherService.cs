using System.Text;
using AmazingChat.Domain.Shared.Models;
using AmazingChat.Domain.Shared.Services;
using RabbitMQ.Client;

namespace AmazingChat.Infra.CrossCutting.Services.RabbitMQ;

public class RabbitMqPublisherService : IRabbitMqPublisherService
{
    private readonly RabbitMqSettings _settings;
    private readonly IModel _channel;

    public RabbitMqPublisherService(RabbitMqSettings settings)
    {
        _settings = settings;

        var factory = new ConnectionFactory
        {
            HostName = settings.HostName,
            Port = settings.Port,
            UserName = settings.UserName,
            Password = settings.Password
        };

        try
        {
            var connection = factory.CreateConnection();
            _channel = connection.CreateModel();

            RabbitMqInitializer.Initiate(_channel, settings);
        }
        catch (Exception ex)
        {
        }
    }


    public bool EnqueueMessage(string message)
    {
        var bodyMessage = Encoding.UTF8.GetBytes(message);

        try
        {
            var props = _channel.CreateBasicProperties();
            props.Persistent = true;

            _channel.BasicPublish(
                _settings.Queue.Exchange.Name,
                _settings.Queue.RoutingKey,
                props,
                bodyMessage
            );
        }
        catch (Exception ex)
        {
            return false;
        }

        return true;
    }
}