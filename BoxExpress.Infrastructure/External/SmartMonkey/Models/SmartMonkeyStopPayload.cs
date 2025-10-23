using System.Text.Json.Serialization;

namespace BoxExpress.Infrastructure.External.SmartMonkey.Models;

public class SmartMonkeyStopPayload
{
    [JsonPropertyName("client_id")]
    public string? ClientId { get; set; }
    [JsonPropertyName("client_external_id")]
    public string? ClientExternalId { get; set; }
    [JsonPropertyName("external_id")]
    public string? ExternalId { get; set; }
    public SmartMonkeyLocationPayload? Location { get; set; }
    [JsonPropertyName("location_details")]
    public string? LocationDetails { get; set; }
    public string? Label { get; set; }
    public string? Status { get; set; }
    public string? Comments { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public string? Url { get; set; }
    [JsonPropertyName("reference_person")]
    public string? ReferencePerson { get; set; }
    public List<string>? Supervisors { get; set; }
    public int? Duration { get; set; }
    public List<string>? Requires { get; set; }
    public string? Cluster { get; set; }
    public decimal? Reward { get; set; }
    [JsonPropertyName("time_windows")]
    public List<List<int>>? TimeWindows { get; set; }
    public decimal? Volume { get; set; }
    public decimal? Weight { get; set; }
    public SmartMonkeyPickupPayload? Pickup { get; set; }
    [JsonPropertyName("max_delivery_time")]
    public int? MaxDeliveryTime { get; set; }
    [JsonPropertyName("route_id")]
    public string? RouteId { get; set; }

    [JsonPropertyName("custom_fields")] 
    public Dictionary<string, string>? CustomFields { get; set; }
    public List<SmartMonkeyTaskPayload>? Tasks { get; set; }
}