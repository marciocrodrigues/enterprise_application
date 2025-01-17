using Microsoft.OpenApi.Models;

namespace NSE.Catalogo.API.Configuration
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
        {
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo()
                {
                    Title = "NerdStore Enterprise Catálogo API",
                    Description = "Esta API faz parte do curso ASP .NET Core Enterprise Application",
                    Contact = new OpenApiContact() { Name = "Marcio", Email = "teste@teste.com.br" },
                    License = new OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/lecenses/MIT") }
                });
            });

            return services;
        }

        public static WebApplication UseSwaggerConfiguration(this WebApplication app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            });

            return app;
        }
    }
}
