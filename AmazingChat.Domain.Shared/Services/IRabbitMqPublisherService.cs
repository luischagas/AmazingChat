namespace AmazingChat.Domain.Interfaces.Services;

public interface IRabbitMqPublisherService
{
    #region Methods

    /// <summary>
    /// Enqueue a message in RabbitMq Service
    /// </summary>
    /// <param name="message"></param>
    /// <returns></returns>
    bool EnqueueMessage(string message);

    #endregion Methods
}