using TradingEngine.Application.Common;
using TradingEngine.Application.Interfaces;
using TradingEngine.Application.Interfaces.Accounts;

namespace TradingEngine.Application.Features.Accounts.Commands;

public sealed record LogoutCommand : ICommand<Result>;

public sealed class LogoutCommandHandler : ICommandHandler<LogoutCommand, Result>
{
    private readonly IUserIdentityRepository _identityRepository;
    private readonly IUserResolverService _userResolverService;

    public LogoutCommandHandler(IUserIdentityRepository identityRepository, IUserResolverService userResolverService)
    {
        _identityRepository = identityRepository;
        _userResolverService = userResolverService;
    }

    public async Task<Result> Handle(LogoutCommand request, CancellationToken cancellationToken)
    {
        var userId = _userResolverService.GetUserId();
        var identity = await _identityRepository.GetByUserIdAsync(userId, cancellationToken);
        
        if (identity != null)
        {
            identity.InvalidateRefreshToken();
            await _identityRepository.UpdateAsync(identity, cancellationToken);
        }

        return Result.Success();
    }
}