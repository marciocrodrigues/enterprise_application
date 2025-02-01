using Microsoft.EntityFrameworkCore;
using NSE.Clientes.API.Data;
using NSE.WebAPI.Core.Identidade;

namespace NSE.Clientes.API.Configuration
{
    public static class ApiConfig
    {
        public static IServiceCollection AddApiConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ClientesContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"));
            });

            services.AddControllers();
            services.AddEndpointsApiExplorer();

            services.AddCors(options =>
            {
                options.AddPolicy("Total", builder =>
                    builder
                    .AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader());
            });

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

        public static WebApplication UseApiConfiguration(this WebApplication app, IWebHostEnvironment env)
        {
            if (app.Environment.IsDevelopment())
            {
                app.UseSwaggerConfiguration();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors("Total");

            app.UseAuthConfiguration();

            app.MapControllers();

            app.Run();

            return app;
        }
    }
}
