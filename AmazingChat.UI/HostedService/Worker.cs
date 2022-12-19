using System.Text;
using AmazingChat.Application.Interfaces;
using AmazingChat.Domain.Shared.Models;
using AmazingChat.Domain.Shared.Services;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;

namespace AmazingChat.UI.HostedService;

public class Worker : BackgroundService
{
    private IConsumerService _consumerService;
    private readonly IRabbitMqConsumerService _rabbitMqConsumerService;
    private readonly IServiceProvider _serviceProvider;

    public Worker(IRabbitMqConsumerService consumerService, IServiceProvider serviceProvider)
    {
        _rabbitMqConsumerService = consumerService;
        _serviceProvider = serviceProvider;
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = _rabbitMqConsumerService.DefineBasicConsumer();

        consumer.Received += (model, ea) => HandleMessageAsync(ea);

        _rabbitMqConsumerService.SetConsumer(consumer);

        return Task.CompletedTask;
    }

    private async Task HandleMessageAsync(BasicDeliverEventArgs ea)
    {
        using (var scope = _serviceProvider.CreateScope())
        {
            _consumerService = scope.ServiceProvider.GetRequiredService<IConsumerService>();

            var brokerMessage = Encoding.Default.GetString(ea.Body.ToArray());

            var channel = _rabbitMqConsumerService.GetChannel();

            try
            {
                var content = JsonConvert.DeserializeObject<MessageStockModel>(brokerMessage);

                if (content != null)
                {
                    var result = await _consumerService.ProcessMessage(content);

                    if (result.Success)
                        channel.BasicAck(ea.DeliveryTag, false);
                    else
                        channel.BasicNack(ea.DeliveryTag, false, ea.Redelivered is false);
                }
            }
            catch (Exception ex)
            {
                channel.BasicNack(ea.DeliveryTag, false, false);
            }

            await Task.Yield();
        }
    }
}