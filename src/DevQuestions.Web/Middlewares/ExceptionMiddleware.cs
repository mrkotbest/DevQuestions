using DevQuestions.Application.Exceptions;
using Shared;
using System.Text.Json;

namespace DevQuestions.Web.Middlewares;

public class ExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionMiddleware> _logger;

    public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext httpContext)
    {
        try
        {
            await _next(httpContext);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(httpContext, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext httpContext, Exception ex)
    {
        _logger.LogError(ex, ex.Message);

        var (code, errors) = ex switch
        {
            BadRequestException => (StatusCodes.Status400BadRequest, JsonSerializer.Deserialize<Error[]>(ex.Message)),

            NotFoundException => (StatusCodes.Status404NotFound, JsonSerializer.Deserialize<Error[]>(ex.Message)),

            _ => (StatusCodes.Status500InternalServerError, [Error.InternalServerError(null, "Somethinmg went wrong..")])
        };

        httpContext.Response.ContentType = "application/json";
        httpContext.Response.StatusCode = code;

        await httpContext.Response.WriteAsJsonAsync(errors);
    }
}

public static class ExceptionMiddlewareExtension
{
    public static IApplicationBuilder UseExceptionMiddleware(this WebApplication app)
    {
        return app.UseMiddleware<ExceptionMiddleware>();
    }
}
