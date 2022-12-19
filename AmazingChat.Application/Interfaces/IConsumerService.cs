using AmazingChat.Domain.Shared.Models;

namespace AmazingChat.Application.Interfaces;

public interface IConsumerService
{
    Task<IAppServiceResponse> ProcessMessage(MessageStockModel request);
}