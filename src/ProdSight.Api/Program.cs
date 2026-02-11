using ProdSight.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddModules(builder.Configuration);
var app = builder.Build();

app.UseModules(builder.Configuration);

app.Run();