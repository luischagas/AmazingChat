using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AmazingChat.Domain.Shared.Services;

public interface IRabbitMqConsumerService
{
    EventingBasicConsumer DefineBasicConsumer();

    IModel GetChannel();

    void SetConsumer(EventingBasicConsumer consumer);
}