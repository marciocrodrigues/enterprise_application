using NSE.WebAPI.Core.Identidade;

namespace NSE.Identidade.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            return services;
        }

        public static WebApplicationBuilder AddSettingsConfiguration(this WebApplicationBuilder builder)
        {
            var env = builder.Environment.EnvironmentName;
            builder.Host.ConfigureAppConfiguration((ctx, build) =>
            {
                build.SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", true, true)
                .AddJsonFile($"appsettings{env}.json", true, true)
                .AddEnvironmentVariables();
            });

            return builder;
        }

        public static WebApplication UseApiConfiguration(this WebApplication app)
        {
            app.UseHttpsRedirection();
            app.UseAuthConfiguration();
            app.MapControllers();

            return app;
        }
    }
}
