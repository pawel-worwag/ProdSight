using System.Text.Json.Serialization;

namespace ProdSight.Api.Shared.Modules;

public class ModuleDescription(string name, string requiredScope, bool loaded = false)
{
    [JsonPropertyName( "name")]
    public string Name { get; set; } = name;
    [JsonPropertyName( "required-scope")]
    public string RequiredScope { get; set; } = requiredScope;
    [JsonPropertyName( "loaded")]
    public bool Loaded { get; set; } = loaded;
}