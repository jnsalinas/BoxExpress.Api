using System.Text.Json.Serialization;

namespace BoxExpress.Infrastructure.External.SmartMonkey.Models;
public class SmartMonkeyCreatePlanPayload
{
    [JsonPropertyName("external_id")]
    public string ExternalId { get; set; }
    public string Label { get; set; }
    public string? Status { get; set; }
    public List<SmartMonkeyStopPayload> Stops { get; set; } = new();
    public List<SmartMonkeyRoutePayload> Routes { get; set; } = new();
    public List<string>? GeoFences { get; set; } = null;
    [JsonPropertyName("execution_date")]
    public DateTime? ExecutionDate { get; set; }
}
