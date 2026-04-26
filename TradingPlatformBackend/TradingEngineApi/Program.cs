using TradingEngine.Api;
using TradingEngine.Api.Common;
using TradingEngine.Api.Hubs;
using TradingEngine.Application;
using TradingEngine.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .AddApiServices(builder.Configuration, builder.Environment)
    .AddApplication()
    .AddInfrastructure(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => Results.Redirect("/swagger"));

app.UseRouting();
app.UseCors(CorsPolicyNames.Frontend);

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<MarketDataHub>("/hubs/market");
app.MapHub<OrderHub>("/hubs/orders");

app.MapControllers();

app.Run();

public partial class Program { }
