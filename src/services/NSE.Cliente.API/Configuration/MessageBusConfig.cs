using NSE.MessageBus;
using NSE.Core.Utils;
using NSE.Clientes.API.Services;

namespace NSE.Clientes.API.Configuration
{
    public static class MessageBusConfig
    {
        public static IServiceCollection AddMessageBusConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddMessageBus(configuration.GetMessageQueueConnection("MessageBus"))
                .AddHostedService<RegistroClienteIntegrationHandler>();

            return services;
        }
    }
}
