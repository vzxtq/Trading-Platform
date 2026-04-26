using System.Text.Json.Serialization;

namespace TradingEngineApi.Common;

/// <summary>
/// Unified API response model for endpoints that return data.
/// Provides consistent response structure across the API.
/// </summary>
public sealed class ApiResponse<T>
{
    public bool Success { get; init; }
    public T? Data { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
    public string Message { get; init; } = string.Empty;
    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public ApiResponse() { }

    [JsonConstructor]
    public ApiResponse(
        bool success,
        T? data,
        string message,
        IReadOnlyList<string> errors)
    {
        Success = success;
        Data = data;
        Message = message;
        Errors = errors;
        Timestamp = DateTime.UtcNow;
    }

    public static ApiResponse<T> SuccessResponse(
        T data,
        string message = "Request completed successfully")
        => new(
            success: true,
            data: data,
            message: message,
            errors: Array.Empty<string>());

    public static ApiResponse<T> FailureResponse(
        IEnumerable<string> errors,
        string message = "Request failed")
        => new(
            success: false,
            data: default,
            message: message,
            errors: errors as IReadOnlyList<string> ?? errors.ToArray());

    public static ApiResponse<T> FailureResponse(
        string error,
        string message = "Request failed")
        => FailureResponse(new[] { error }, message);
}

public sealed class ApiResponse
{
    public bool Success { get; init; }

    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();

    public string Message { get; init; } = string.Empty;

    public DateTime Timestamp { get; init; } = DateTime.UtcNow;

    public ApiResponse() { }

    [JsonConstructor]
    public ApiResponse(
        bool success,
        string message,
        IReadOnlyList<string> errors)
    {
        Success = success;
        Message = message;
        Errors = errors;
        Timestamp = DateTime.UtcNow;
    }

    public static ApiResponse SuccessResponse(
        string message = "Request completed successfully")
        => new(
            success: true,
            message: message,
            errors: Array.Empty<string>());

    public static ApiResponse FailureResponse(
        IEnumerable<string> errors,
        string message = "Request failed")
        => new(
            success: false,
            message: message,
            errors: errors as IReadOnlyList<string> ?? errors.ToArray());

    public static ApiResponse FailureResponse(
        string error,
        string message = "Request failed")
        => FailureResponse(new[] { error }, message);
}