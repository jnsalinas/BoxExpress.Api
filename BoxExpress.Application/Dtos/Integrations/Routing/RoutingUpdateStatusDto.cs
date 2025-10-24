using System.Text.Json.Serialization;

namespace BoxExpress.Application.Dtos.Integrations.Routing;

public class RoutingUpdateStatusDto
{

    [JsonPropertyName("data")]
    public RoutingDataDto Data { get; set; }
}

public class RoutingDataDto
{
    [JsonPropertyName("label")]
    public string Label { get; set; }

    [JsonPropertyName("status")]
    public string Status { get; set; }

    [JsonPropertyName("reports")]
    public List<ReportDto> Reports { get; set; }
}

public class ReportDto
{
    [JsonPropertyName("images")]
    public List<object> Images { get; set; }

    [JsonPropertyName("comments")]
    public string Comments { get; set; }

}

public class ImageDto
{
    [JsonPropertyName("url")]
    public string Url { get; set; }

    [JsonPropertyName("id")]
    public string Id { get; set; }
}