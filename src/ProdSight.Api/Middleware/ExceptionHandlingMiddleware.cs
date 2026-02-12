using ProdSight.Api.Shared.DTOs.Errors;
using ProdSight.Api.Shared.Exceptions;

namespace ProdSight.Api.Middleware;

public class ExceptionHandlingMiddleware(ILogger<ExceptionHandlingMiddleware> logger, IHostEnvironment environment)
    : IMiddleware
{
    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (AppException ex)
        {
            logger.LogWarning(ex, "Application exception occurred");
            context.Response.StatusCode = ex.StatusCode;
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse
            {
                Error = ex.ErrorType,
                Message = ex.Message
            };
            await context.Response.WriteAsJsonAsync(response);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occurred");
            context.Response.StatusCode = 500;
            context.Response.ContentType = "application/json";
            var response = new ErrorResponse
            {
                Error = "Internal server error",
                Message = environment.IsDevelopment() ? ex.Message : "An error occurred"
            };
            await context.Response.WriteAsJsonAsync(response);
        }
    }
}