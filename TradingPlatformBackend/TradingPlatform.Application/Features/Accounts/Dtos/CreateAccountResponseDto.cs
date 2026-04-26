namespace TradingEngine.Application.Features.Accounts.Dtos;

public sealed class CreateAccountResponseDto
{
    public Guid AccountId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public decimal Balance { get; set; }
}
