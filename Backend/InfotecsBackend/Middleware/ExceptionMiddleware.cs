using System.Net;
using System.Text.Json;
using InfotecsBackend.Errors.Exceptions;

namespace InfotecsBackend.Middleware;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        _logger.LogError(exception, "An error occurred");

        var response = context.Response;
        response.ContentType = "application/json";

        var errorResponse = new
        {
            message = exception.Message,
            statusCode = (int)HttpStatusCode.InternalServerError
        };

        switch (exception)
        {
            case AppException appEx:
                errorResponse = new { message = appEx.Message, statusCode = appEx.StatusCode };
                response.StatusCode = appEx.StatusCode;
                break;
                
            default:
                errorResponse = new { message = "Internal server error", statusCode = 500 };
                response.StatusCode = 500;
                break;
        }

        var jsonResponse = JsonSerializer.Serialize(errorResponse);
        await response.WriteAsync(jsonResponse);
    }
}