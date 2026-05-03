using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using TradingEngine.Application.Features.Accounts.Commands;
using TradingEngine.Application.Features.Accounts.Dtos;
using TradingEngine.Application.Features.Orders.Commands;
using TradingEngine.Application.Features.Orders.Dtos;
using TradingEngine.Domain.Enums;
using TradingEngineApi.Common;
using Xunit;

namespace TradingPlatform.IntegrationTests;

public class SellOrderValidationTests : IClassFixture<TradingPlatformFactory>
{
    private readonly TradingPlatformFactory _factory;
    private readonly HttpClient _client;

    public SellOrderValidationTests(TradingPlatformFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task PlaceSellOrder_WithoutPosition_ShouldFail()
    {
        await _factory.InitializeDatabaseAsync();
        var token = await RegisterAndLoginAsync($"seller_none_{Guid.NewGuid()}@test.com", "Seller123!", "Seller", "None");

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var sellCommand = new PlaceOrderCommand
        {
            Symbol = "BTCUSD",
            Price = 50000,
            Quantity = 1,
            Side = OrderSide.Sell
        };

        var response = await _client.PostAsJsonAsync("/api/orders", sellCommand);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<PlaceOrderResponseDto>>();
        content!.Success.Should().BeFalse();
        content.Errors.Should().Contain("Insufficient position");
    }

    [Fact]
    public async Task PlaceSellOrder_WithInsufficientQuantity_ShouldFail()
    {
        await _factory.InitializeDatabaseAsync();
        var email = $"seller_insuff_{Guid.NewGuid()}@test.com";
        var token = await RegisterAndLoginAsync(email, "Seller123!", "Seller", "Insuff");

        // Seed small position
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<TradingEngine.Infrastructure.Persistence.TradingDbContext>();
            var user = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstAsync(context.UserAccounts, a => a.Email == email);
            var position = TradingEngine.Domain.Entities.PositionDomain.Create(user.Id, new TradingEngine.Domain.ValueObjects.Symbol("BTCUSD"), new TradingEngine.Domain.ValueObjects.Quantity(5), 40000);
            context.Positions.Add(position!);
            await context.SaveChangesAsync();
        }

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var sellCommand = new PlaceOrderCommand
        {
            Symbol = "BTCUSD",
            Price = 50000,
            Quantity = 10,
            Side = OrderSide.Sell
        };

        var response = await _client.PostAsJsonAsync("/api/orders", sellCommand);
        
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        var content = await response.Content.ReadFromJsonAsync<ApiResponse<PlaceOrderResponseDto>>();
        content!.Success.Should().BeFalse();
        content.Errors.Should().Contain("Insufficient position");
    }

    private async Task<string> RegisterAndLoginAsync(string email, string password, string first, string last)
    {
        var registerCommand = new RegisterUserCommand(email, password, first, last, 100000, Currency.USD);
        await _client.PostAsJsonAsync("/api/accounts/register", registerCommand);

        var loginCommand = new LoginCommand(email, password);
        var logResp = await _client.PostAsJsonAsync("/api/accounts/login", loginCommand);
        var content = await logResp.Content.ReadFromJsonAsync<ApiResponse<LoginResponseDto>>();
        return content!.Data!.Token;
    }
}
