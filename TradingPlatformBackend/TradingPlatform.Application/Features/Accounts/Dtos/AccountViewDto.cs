using TradingEngine.Domain.ValueObjects;

namespace TradingEngine.Application.Features.Accounts.Dtos
{
    public class AccountViewDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string Name { get; set; }
        public required Money Balance { get; set; }
        public required Money ReservedBalance { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
