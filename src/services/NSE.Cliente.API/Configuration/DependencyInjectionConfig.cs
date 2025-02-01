using NSE.Clientes.API.Data;

namespace NSE.Clientes.API.Configuration
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection RegisterServices(this IServiceCollection services)
        {
            services.AddScoped<ClientesContext>();
            return services;
        }
    }
}
