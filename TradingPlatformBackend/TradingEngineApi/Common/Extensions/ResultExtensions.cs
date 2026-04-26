using Microsoft.AspNetCore.Mvc;
using TradingEngineApi.Common;
using TradingEngine.Application.Common;

namespace TradingEngineApi.Extensions;

/// <summary>
/// Extension methods for converting application Result objects to API responses.
/// Provides seamless integration between domain logic and HTTP responses.
/// </summary>
public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(ApiResponse.SuccessResponse());
        }

        return new BadRequestObjectResult(
            ApiResponse.FailureResponse(result.Errors, "Request failed"));
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess)
        {
            return new OkObjectResult(
                ApiResponse<T>.SuccessResponse(result.Value!));
        }

        return new BadRequestObjectResult(
            ApiResponse<T>.FailureResponse(result.Errors, "Request failed"));
    }

    public static OkObjectResult ToOkResult<T>(
        this Result result,
        T data,
        string message = "Request completed successfully")
    {
        return new OkObjectResult(
            ApiResponse<T>.SuccessResponse(data, message));
    }

    public static OkObjectResult ToOkResult<T>(
        this Result<T> result,
        string message = "Request completed successfully")
    {
        return new OkObjectResult(
            ApiResponse<T>.SuccessResponse(result.Value!, message));
    }

    public static CreatedAtActionResult ToCreatedAtActionResult<T>(
        this Result<T> result,
        string actionName,
        string? controllerName = null,
        object? routeValues = null,
        string message = "Resource created successfully")
    {
        var response = ApiResponse<T>.SuccessResponse(result.Value!, message);

        return new CreatedAtActionResult(
            actionName,
            controllerName,
            routeValues,
            response);
    }

    public static BadRequestObjectResult ToBadRequestResult(
        this Result result,
        string message = "Request failed")
    {
        return new BadRequestObjectResult(
            ApiResponse.FailureResponse(result.Errors, message));
    }

    public static BadRequestObjectResult ToBadRequestResult<T>(
        this Result<T> result,
        string message = "Request failed")
    {
        return new BadRequestObjectResult(
            ApiResponse<T>.FailureResponse(result.Errors, message));
    }

    public static NotFoundObjectResult ToNotFoundResult(
        this Result result,
        string message = "Resource not found")
    {
        return new NotFoundObjectResult(
            ApiResponse.FailureResponse(result.Errors, message));
    }

    public static NotFoundObjectResult ToNotFoundResult<T>(
        this Result<T> result,
        string message = "Resource not found")
    {
        return new NotFoundObjectResult(
            ApiResponse<T>.FailureResponse(result.Errors, message));
    }
}