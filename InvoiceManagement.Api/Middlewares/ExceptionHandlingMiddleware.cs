using System.Net;
using System.Text.Json;

namespace InvoiceManagement.Api.Middlewares;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
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
            _logger.LogError(ex, "Unhandled exception on {Method} {Path}", context.Request.Method, context.Request.Path);
            await HandleExceptionAsync(context, ex);
        }
    }

    private static async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var (statusCode, message) = exception switch
        {
            ArgumentException        => (HttpStatusCode.BadRequest,            exception.Message),
            UnauthorizedAccessException => (HttpStatusCode.Unauthorized,       "Unauthorized"),
            KeyNotFoundException     => (HttpStatusCode.NotFound,              exception.Message),
            InvalidOperationException => (HttpStatusCode.UnprocessableEntity,  exception.Message),
            _                        => (HttpStatusCode.InternalServerError,   "An unexpected error occurred")
        };

        context.Response.StatusCode  = (int)statusCode;
        context.Response.ContentType = "application/json";

        var response = new
        {
            status  = (int)statusCode,
            error   = message
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
}
