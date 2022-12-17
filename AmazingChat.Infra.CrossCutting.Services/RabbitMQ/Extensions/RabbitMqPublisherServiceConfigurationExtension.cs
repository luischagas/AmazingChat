using AmazingChat.Domain.Interfaces.Services;
using AmazingChat.Domain.Shared.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AmazingChat.Infra.CrossCutting.Services.RabbitMQ.Extensions
{
    public static class RabbitMqPublisherServiceConfigurationExtension
    {
        #region Methods

        public static IServiceCollection AddRabbitMqPublisherService(this IServiceCollection services, IConfiguration configuration)
        {
            var rabbitMqSettings = new RabbitMqSettings();

            configuration.GetSection("RabbitMqSettings").Bind(rabbitMqSettings);

            services.AddSingleton(rabbitMqSettings);

            services.AddSingleton<IRabbitMqPublisherService, RabbitMqPublisherService>();

            return services;
        }

        #endregion Methods
    }
}