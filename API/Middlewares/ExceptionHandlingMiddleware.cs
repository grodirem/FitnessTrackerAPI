using BLL.DTOs;
using BLL.Exceptions;
using System.Net;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace LibraryWebApp_v2;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;
    private readonly bool _isDevelopment;

    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger,
        IHostEnvironment env)
    {
        _next = next;
        _logger = logger;
        _isDevelopment = env.IsDevelopment();
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
        var (statusCode, message) = GetStatusCodeAndMessage(exception);

        _logger.LogError(exception, "Request processing error: {Message}", exception.Message);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var response = new ErrorResponse
        {
            StatusCode = statusCode,
            Message = message,
            Details = _isDevelopment ? exception.Message : null,
            StackTrace = _isDevelopment ? exception.StackTrace : null
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
        }));
    }

    private (HttpStatusCode statusCode, string message) GetStatusCodeAndMessage(Exception exception)
    {
        _logger.LogDebug("Handling exception of type {ExceptionType}", exception.GetType().Name);

        return exception switch
        {
            ValidationException => (HttpStatusCode.BadRequest, "Validation error: " + exception.Message),
            NotFoundException => (HttpStatusCode.NotFound, "Resource not found: " + exception.Message),
            UnauthorizedException => (HttpStatusCode.Unauthorized, "Authorization failed: " + exception.Message),
            ConflictException => (HttpStatusCode.Conflict, "Conflict detected: " + exception.Message),
            InvalidOperationException => (HttpStatusCode.BadRequest, "Invalid operation: " + exception.Message),
        };
    }
}