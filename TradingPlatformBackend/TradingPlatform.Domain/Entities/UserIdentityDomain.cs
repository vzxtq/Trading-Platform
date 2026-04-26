using TradingEngine.Domain.Common;

namespace TradingEngine.Domain.Entities;

public sealed class UserIdentityDomain : BaseEntity
{
    public Guid UserId { get; private set; }
    public string PasswordHash { get; private set; } = null!;

    private UserIdentityDomain() { }

    public UserIdentityDomain(Guid userId, string passwordHash)
    {
        Id = Guid.NewGuid();
        UserId = userId;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
    }
}
