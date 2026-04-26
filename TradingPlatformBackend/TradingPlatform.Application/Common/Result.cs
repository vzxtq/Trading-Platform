namespace TradingEngine.Application.Common;

/// <summary>
/// Encapsulates success/failure state and error information.
/// </summary>
public class Result
{
    public bool IsSuccess { get; }

    public bool IsFailure => !IsSuccess;

    public IReadOnlyList<string> Errors { get; }

    protected Result(bool isSuccess, IReadOnlyList<string> errors)
    {
        IsSuccess = isSuccess;
        Errors = errors;
    }

    public static Result Success()
        => new Result(true, Array.Empty<string>());

    public static Result Failure(params string[] errors)
        => new Result(false, errors);
}

public class Result<T> : Result
{
    public T? Value { get; }

    private Result(T? value, bool isSuccess, IReadOnlyList<string> errors)
        : base(isSuccess, errors)
    {
        Value = value;
    }

    public static Result<T> Success(T value)
        => new Result<T>(value, true, Array.Empty<string>());

    public static new Result<T> Failure(params string[] errors)
        => new Result<T>(default, false, errors);
}