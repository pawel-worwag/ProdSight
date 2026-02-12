namespace ProdSight.Api.Shared.Exceptions;

public abstract class AppException(string message, int statusCode, string errorType) : Exception(message)
{
    public int StatusCode { get; } = statusCode;
    public string ErrorType { get; } = errorType;
}