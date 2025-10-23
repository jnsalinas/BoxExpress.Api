namespace BoxExpress.Infrastructure.External.SmartMonkey.Models;

public class SmartMonkeyRoutePayload
{
    public string VehicleId { get; set; }
    public string ExternalId { get; set; }
    public SmartMonkeyLocationPayload StartLocation { get; set; }
    public SmartMonkeyLocationPayload EndLocation { get; set; }
    public string Label { get; set; }
    public string Phone { get; set; }
    public string Comments { get; set; }
    public string Email { get; set; }
    public string Plate { get; set; }
    public string VehicleModel { get; set; }
    public string Color { get; set; }
    public string Brand { get; set; }
    public bool IsLocked { get; set; }
    public List<string> GeoFences { get; set; } = new();
    public List<int> TimeWindow { get; set; } = new();
    public decimal MaxDistance { get; set; }
    public decimal MinDistance { get; set; }
    public decimal MaxTime { get; set; }
    public decimal MinTime { get; set; }
    public decimal MaxWeight { get; set; }
    public decimal MaxVolume { get; set; }
    public int MaxServices { get; set; }
    public List<string> Provides { get; set; } = new();
    public Dictionary<string, string> CustomFields { get; set; } = new();
    public decimal PricePerDistance { get; set; }
    public decimal PricePerMinute { get; set; }
    public string Status { get; set; }
}