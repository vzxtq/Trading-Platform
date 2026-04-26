using TradingEngine.Domain.Entities;

namespace TradingEngine.Application.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(UserIdentityDomain identity);
}
