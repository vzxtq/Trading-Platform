using TradingEngine.Application.Common;

namespace TradingEngine.Application.Features.Accounts.Commands;

/// <summary>
/// Command to deposit funds into a user account.
/// </summary>
public class DepositCommand : ICommand<DepositResponse>
{
    public Guid UserId { get; set; }
    public decimal Amount { get; set; }
}

public class DepositResponse
{
    public Guid UserId { get; set; }
    public decimal NewBalance { get; set; }
    public bool Success { get; set; }
}

//should be implemented in the nearest future