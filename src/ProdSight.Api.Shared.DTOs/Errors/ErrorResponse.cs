using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.DTOs.Errors;

public class ErrorResponse
{
    [JsonPropertyName("error")]
    public required string Error { get; set; }

    [JsonPropertyName("message")]
    public required string Message { get; set; }
    
    [JsonPropertyName("details")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string? Details { get; set; }
    
    
}