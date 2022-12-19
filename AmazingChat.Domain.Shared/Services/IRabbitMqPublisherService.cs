namespace AmazingChat.Domain.Shared.Services;

public interface IRabbitMqPublisherService
{
    bool EnqueueMessage(string message);
}