using NSE.WebApp.MVC.Configuration;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddIndetityConfiguration();

builder.Services.AddMvcConfiguration();

var app = builder.Build();

app.UseMvcConfiguration();

app.Run();
