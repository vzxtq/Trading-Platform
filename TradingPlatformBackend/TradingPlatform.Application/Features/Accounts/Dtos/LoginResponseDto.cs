namespace TradingEngine.Application.Features.Accounts.Dtos;

public sealed class LoginResponseDto
{
    public string Token { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = string.Empty;
    public long ExpiresAt { get; set; }
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
}
