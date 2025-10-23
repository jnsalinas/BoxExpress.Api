namespace BoxExpress.Infrastructure.External.SmartMonkey.Models;
public class SmartMonkeyCreatePlanResponse
{
    public string Id { get; set; }
    public string OrganizationId { get; set; }
    public string ProjectId { get; set; }
    public int CompletedStops { get; set; }
    public int CanceledStops { get; set; }
    public int PendingStops { get; set; }
    public int IncompleteStops { get; set; }
    public int TotalStops { get; set; }
    public int TotalRoutes { get; set; }
    public string Label { get; set; }
    public DateTime ExecutionDate { get; set; }
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
    public DateTime DeletedAt { get; set; }
    public bool Deleted { get; set; }
}