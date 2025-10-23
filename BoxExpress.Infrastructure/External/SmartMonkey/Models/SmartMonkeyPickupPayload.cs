using System.Text.Json.Serialization;

namespace BoxExpress.Infrastructure.External.SmartMonkey.Models;

public class SmartMonkeyPickupPayload
{
    public string ClientId { get; set; }
    public string ClientExternalId { get; set; }

    [JsonPropertyName("external_id")]
    public string ExternalId { get; set; }
    public SmartMonkeyLocationPayload Location { get; set; }

    [JsonPropertyName("location_details")]
    public string LocationDetails { get; set; }
    public string Label { get; set; }
    public string Comments { get; set; }
    public string Phone { get; set; }
    public string Email { get; set; }
    public string Url { get; set; }

    [JsonPropertyName("reference_person")]
    public string ReferencePerson { get; set; }
    public string RouteId { get; set; }
    public int Duration { get; set; }
    public decimal Reward { get; set; }
    public decimal Price { get; set; }
    public List<string> Requires { get; set; } = new();
    public string Cluster { get; set; }
    public List<List<int>> TimeWindows { get; set; } = new();
    public decimal Volume { get; set; }
    public decimal Weight { get; set; }
    public int MaxDeliveryTime { get; set; }
    public string Status { get; set; }
    public List<SmartMonkeyTaskPayload> Tasks { get; set; } = new();
    public Dictionary<string, string> CustomFields { get; set; } = new();
    public List<string> Supervisors { get; set; } = new();
}