using System.Text.Json.Serialization;

namespace BoxExpress.Infrastructure.External.SmartMonkey.Models;

public class SmartMonkeyTaskPayload
{
    public string? Label { get; set; }
    public string? Comments { get; set; }
    public string? Barcode { get; set; }
    public string? Status { get; set; }
    [JsonPropertyName("custom_fields")]
    public Dictionary<string, string>? CustomFields { get; set; }
}