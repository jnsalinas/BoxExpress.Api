public class RoutingStopDto
{
    public string Label { get; set; }
    public RoutingLocationDto Location { get; set; }
    public string Comments { get; set; }
    public string ExternalId { get; set; }
    public string LocationDetails { get; set; }
    public string Status { get; set; } = "pending";
    public string Email { get; set; }
    public string ReferencePerson { get; set; }
    public string Phone { get; set; } //todo revisar colombia o mx poner prefix +57 o +52
    public string Price { get; set; }
    public Dictionary<string, string> CustomFields { get; set; } = new();

}