using AmazingChat.Application.Models;

namespace AmazingChat.Application.Interfaces;

public interface IStockBotService
{
    Task<IAppServiceResponse> ProcessCommand(CommandViewModel request);
}