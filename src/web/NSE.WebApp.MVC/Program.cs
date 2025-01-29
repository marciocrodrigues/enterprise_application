using NSE.WebApp.MVC.Configuration;

var builder = WebApplication.CreateBuilder(args);

var env = builder.Environment.EnvironmentName;
builder.Host.ConfigureAppConfiguration((ctx, build) =>
{
    build.SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("appsettings.json", true, true)
    .AddJsonFile($"appsettings{env}.json", true, true)
    .AddEnvironmentVariables();
});

builder.Services.AddIndetityConfiguration();

builder.Services.AddMvcConfiguration(builder.Configuration);

builder.Services.RegisterServices();

var app = builder.Build();

app.UseMvcConfiguration();

app.Run();
