using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace AmazingChat.Domain.Shared.Services;

public interface IRabbitMqConsumerService
{
    #region Methods
    
    EventingBasicConsumer DefineBasicConsumer();
    
    IModel GetChannel();
    
    void SetConsumer(EventingBasicConsumer consumer);

    #endregion Methods
}