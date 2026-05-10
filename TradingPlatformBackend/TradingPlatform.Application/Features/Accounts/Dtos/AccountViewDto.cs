using TradingEngine.Application.Features.Accounts.Dtos;

namespace TradingEngine.Application.Features.Accounts.Dtos
{
    public class AccountViewDto
    {
        public Guid Id { get; set; }
        public required string Email { get; set; }
        public required string FullName { get; set; }
        public required string FirstName { get; set; }
        public required string LastName { get; set; }
        public required MoneyDto Balance { get; set; }
        public required MoneyDto ReservedBalance { get; set; }
        public required MoneyDto AvailableBalance { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
