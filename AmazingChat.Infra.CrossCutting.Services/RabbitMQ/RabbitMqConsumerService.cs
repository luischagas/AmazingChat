using AmazingChat.Domain.Shared.Models;
using AmazingChat.Domain.Shared.Services;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AmazingChat.Infra.CrossCutting.Services.RabbitMQ;

public class RabbitMqConsumerService : IRabbitMqConsumerService
{
    private readonly RabbitMqSettings _settings;
    private readonly IModel _channel;
    
    public RabbitMqConsumerService(RabbitMqSettings settings)
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
            _settings.Queue.Name,
            false,
            consumer
        );
    }
}