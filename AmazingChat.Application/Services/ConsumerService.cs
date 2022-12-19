using AmazingChat.Application.Common;
using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Models;
using AmazingChat.Domain.Shared.Models;
using Microsoft.Extensions.Options;

namespace AmazingChat.Application.Services;

public class ConsumerService : IConsumerService
{
    private readonly SignalRConfigurations _signalRConfigurations;
    private readonly IMessageService _messageService;

    public ConsumerService(IOptions<SignalRConfigurations> signalRConfigurations,
        IMessageService messageService)
    {
        _messageService = messageService;
        _signalRConfigurations = signalRConfigurations.Value;
    }


    public async Task<IAppServiceResponse> ProcessMessage(MessageStockModel request)
    {
        if (string.IsNullOrEmpty(request.Message))
            return await Task.FromResult(new AppServiceResponse<object>(null, "Error to send Stock Message", false));

        var result = await _messageService.Create(new MessageViewModel
        {
            Message = request.Message,
            User = _signalRConfigurations.StockUser,
            Room = request.Room
        });

        if (result.Success)
            return await Task.FromResult(new AppServiceResponse<object>(null, "Stock Message Created Successfully", true));


        return await Task.FromResult(new AppServiceResponse<object>(null, "Error to send Stock Message", false));
    }
}