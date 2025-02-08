using NSE.Carrinho.API.Configuration;
using NSE.WebAPI.Core.Identidade;

var builder = WebApplication.CreateBuilder(args);

builder.AddSettingsConfiguration();

builder.Services.AddApiConfiguration(builder.Configuration);

builder.Services.AddJwtConfiguration(builder.Configuration);

//builder.Services.AddMediatR(cfg =>
//{
//    cfg.RegisterServicesFromAssemblyContaining<Program>();
//    cfg.Lifetime = ServiceLifetime.Scoped;
//});

builder.Services.RegisterServices();

builder.Services.AddSwaggerConfiguration();

var app = builder.Build();

app.UseApiConfiguration(app.Environment);
