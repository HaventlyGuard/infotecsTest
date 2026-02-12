using System.Net;
using System.Text.Json;
using InfotecsBackend.Errors.Exceptions;
using Microsoft.AspNetCore.Diagnostics;

namespace InfotecsBackend.Middleware;

public class ExceptionMiddleware : IExceptionHandler
{
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(ILogger<ExceptionMiddleware> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(HttpContext context, Exception exception, CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "An error occurred");

        var response = context.Response;
        response.ContentType = "application/json";

        var (message, statusCode) = exception switch
        {
            AppException appEx => (appEx.Message, appEx.StatusCode),
            _ => ("Internal server error", 500)
        };

        response.StatusCode = statusCode;

        var errorResponse = new
        {
            message,
            statusCode
        };

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(jsonResponse, cancellationToken);
        
        return true;
    }
}