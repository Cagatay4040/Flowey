using Flowey.CORE.Result.Concrete;
using Flowey.SHARED.Constants;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Global Exception Caught: {Message}", exception.Message);

        var (statusCode, displayMessage) = exception switch
        {
            KeyNotFoundException => (StatusCodes.Status404NotFound, ExceptionMessages.NotFound),
            UnauthorizedAccessException => (StatusCodes.Status401Unauthorized, ExceptionMessages.Unauthorized),
            FluentValidation.ValidationException => (StatusCodes.Status400BadRequest, ExceptionMessages.ValidationFailed),

            _ => (StatusCodes.Status500InternalServerError, ExceptionMessages.InternalServerError)
        };

        httpContext.Response.StatusCode = statusCode;
        httpContext.Response.ContentType = "application/json";

        var resultResponse = new Result(
            ResultStatus.Error,
            displayMessage
        );

        await httpContext.Response.WriteAsJsonAsync(resultResponse, cancellationToken);

        return true;
    }
}