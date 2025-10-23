using System.Text.Json.Serialization;

namespace BoxExpress.Infrastructure.External.SmartMonkey.Models;

public class SmartMonkeyLocationPayload
{
    public string? LocationId { get; set; }
    public string? Label { get; set; }
    public string? Country { get; set; }
    [JsonPropertyName("country_code")]
    public string? CountryCode { get; set; }
    public string? State { get; set; }
    public string? City { get; set; }
    public string? Street { get; set; }
    [JsonPropertyName("postal_code")]
    public string? PostalCode { get; set; }
    public string? Comments { get; set; }
    [JsonPropertyName("partial_match")]
    public bool? PartialMatch { get; set; }
    public double? Lat { get; set; }
    public double? Lng { get; set; }
}