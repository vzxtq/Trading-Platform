using MediatR;

namespace TradingEngine.Application.Common;

/// <summary>
/// Marker interface for commands without response.
/// </summary>
public interface ICommand : IRequest
{
}

/// <summary>
/// Marker interface for commands with response.
/// </summary>
public interface ICommand<out TResponse> : IRequest<TResponse>
{
}

public interface ICommandHandler<in TCommand> : IRequestHandler<TCommand>
    where TCommand : ICommand
{
}

public interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}