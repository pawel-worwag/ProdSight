using ProdSight.Api.Shared.DTOs.Errors;

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