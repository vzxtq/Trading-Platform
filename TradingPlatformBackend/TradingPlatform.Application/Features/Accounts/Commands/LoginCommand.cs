using TradingEngine.Application.Common;
using TradingEngine.Application.Features.Accounts.Dtos;
using TradingEngine.Application.Interfaces;
using TradingEngine.Application.Interfaces.Accounts;
using TradingEngine.Domain.Interfaces;

namespace TradingEngine.Application.Features.Accounts.Commands;

public sealed record LoginCommand(string Email, string Password) : ICommand<Result<LoginResponseDto>>;

public sealed class LoginCommandHandler : ICommandHandler<LoginCommand, Result<LoginResponseDto>>
{
    private readonly IUserIdentityRepository _identityRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator _tokenGenerator;

    public LoginCommandHandler(
        IUserIdentityRepository identityRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenGenerator tokenGenerator)
    {
        _identityRepository = identityRepository;
        _passwordHasher = passwordHasher;
        _tokenGenerator = tokenGenerator;
    }

    public async Task<Result<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var identity = await _identityRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (identity == null)
        {
            return Result<LoginResponseDto>.Failure("Invalid email or password.");
        }

        if (!_passwordHasher.VerifyPassword(request.Password, identity.PasswordHash))
        {
            return Result<LoginResponseDto>.Failure("Invalid email or password.");
        }

        var token = _tokenGenerator.GenerateToken(identity);

        return Result<LoginResponseDto>.Success(new LoginResponseDto
        {
            Token = token,
            UserId = identity.UserId
        });
    }
}
