namespace ProdSight.Api.Shared.DTOs.Errors;

public class ErrorResponse
{
    public required string Error { get; set; }
    public required string Message { get; set; }
}