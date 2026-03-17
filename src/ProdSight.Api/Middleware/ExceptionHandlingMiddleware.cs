using Microsoft.EntityFrameworkCore;
using Npgsql;
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
        catch (DbUpdateException ex) when (ex.InnerException is PostgresException pg)
        {
            logger.LogError(ex, "Unhandled exception occurred");
            context.Response.StatusCode = 400;
            context.Response.ContentType = "application/json";
            var (message, details) = MapPgExceptionToMessage(pg);
            var response = new ErrorResponse
            {
                Error = "DbUpdateException",
                Message = message,
                Details = details
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

    private (string message, string? details) MapPgExceptionToMessage(PostgresException ex)
    {
        switch (ex.SqlState)
        {
            case "23503":
                return ("Foreign key constraint violation", 
                    $"Table: {ex.TableName}, Constraint: {ex.ConstraintName}");
        }

        return (ex.MessageText, null);
    }
}