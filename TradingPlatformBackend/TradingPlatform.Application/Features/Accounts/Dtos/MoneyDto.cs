using TradingEngine.Domain.Enums;

namespace TradingEngine.Application.Features.Accounts.Dtos;

public class MoneyDto
{
    public decimal Amount { get; set; }
    public Currency Currency { get; set; }
}
