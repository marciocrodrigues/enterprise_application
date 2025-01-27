using NSE.WebApp.MVC.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIndetityConfiguration();

builder.Services.AddMvcConfiguration();

builder.Services.RegisterServices();

var app = builder.Build();

app.UseMvcConfiguration();

app.Run();
