using System.Globalization;
using AmazingChat.Application.Common;
using AmazingChat.Application.Interfaces;
using AmazingChat.Application.Models;
using AmazingChat.Domain.Interfaces.Services;
using AmazingChat.Domain.Shared.Models;
using AmazingChat.Domain.Shared.Notifications;
using AmazingChat.Domain.Shared.Services;
using AmazingChat.Domain.Shared.UnitOfWork;
using CsvHelper;
using Microsoft.Extensions.Options;
using RestSharp;

namespace AmazingChat.Application.Services;

public class StockBotService : AppService, IStockBotService
{
    #region Fields

    private readonly SignalRConfigurations _signalRConfigurations;
    private readonly ICommunicationRestService _communicationRestService;
    private readonly IRabbitMqPublisherService _rabbitMqPublisherService;

    #endregion

    #region Constructors

    public StockBotService(IUnitOfWork unitOfWork,
        INotifier notifier,
        IOptions<SignalRConfigurations> signalRConfigurations,
        ICommunicationRestService communicationRestService,
        IRabbitMqPublisherService rabbitMqPublisherService)
        : base(unitOfWork, notifier)
    {
        _communicationRestService = communicationRestService;
        _rabbitMqPublisherService = rabbitMqPublisherService;
        _signalRConfigurations = signalRConfigurations.Value;
    }

    #endregion Constructors

    #region Public Methods

    public async Task<IAppServiceResponse> ProcessCommand(CommandViewModel request)
    {
        var path = string.Format($"{_signalRConfigurations.QueryString}", request.Command);

        var result = _communicationRestService.SendRequest(_signalRConfigurations.UrlStockBot, path, Method.Get);

        if (result.IsSuccessful)
        {
            var stockQuote = ConvertToString(result.Content);

            if (string.IsNullOrEmpty(stockQuote) is false)
            {
                var queued = _rabbitMqPublisherService.EnqueueMessage(stockQuote);

                if (queued)
                    return await Task.FromResult(new AppServiceResponse<string>(stockQuote, "Stock obtained Successfully", true));
            }
            
            return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to obtain Stock", false));
           
        }

        return await Task.FromResult(new AppServiceResponse<ICollection<Notification>>(GetAllNotifications(), "Error to obtain Stock", false));
    }

    private string ConvertToString(string value)
    {
        using var reader = new StringReader(value);
        try
        {
            using var csvReader = new CsvReader(reader, CultureInfo.InvariantCulture);

            var records = csvReader.GetRecords<StockSheetModel>().ToList();

            if (records.Any())
            {
                var stock = records.First();

                return $"{stock.Symbol} quote is ${stock.Close:N2} per share";
            }

            return string.Empty;
        }
        catch (Exception e)
        {
            return string.Empty;
        }
    }

    #endregion Public Methods
}