using System.Text.Json.Serialization;

namespace BoxExpress.Infrastructure.External.SmartMonkey.Models;

public class SmartMonkeyCreatePlanResponse
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("organization_id")]
    public string OrganizationId { get; set; }

    [JsonPropertyName("project_id")]
    public string ProjectId { get; set; }

    [JsonPropertyName("completed_stops")]
    public int CompletedStops { get; set; }

    [JsonPropertyName("canceled_stops")]
    public int CanceledStops { get; set; }

    [JsonPropertyName("pending_stops")]
    public int PendingStops { get; set; }

    [JsonPropertyName("incomplete_stops")]
    public int IncompleteStops { get; set; }

    [JsonPropertyName("total_stops")]
    public int TotalStops { get; set; }

    [JsonPropertyName("total_routes")]
    public int TotalRoutes { get; set; }

    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("created_by")]
    public string CreatedBy { get; set; }

    [JsonPropertyName("created_at")]
    public DateTime CreatedAt { get; set; }

    [JsonPropertyName("updated_at")]
    public DateTime UpdatedAt { get; set; }

    [JsonPropertyName("deleted_at")]
    public DateTime? DeletedAt { get; set; }

    [JsonPropertyName("deleted")]
    public bool Deleted { get; set; }
}
