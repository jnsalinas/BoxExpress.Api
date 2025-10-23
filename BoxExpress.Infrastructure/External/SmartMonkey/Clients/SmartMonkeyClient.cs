using System.Text;
using Microsoft.Extensions.Configuration;
using BoxExpress.Application.Configurations;
using System.Net.Http.Headers;
using BoxExpress.Application.Interfaces;
using BoxExpress.Application.Dtos.Integrations.Routing;
using AutoMapper;
using BoxExpress.Infrastructure.External.SmartMonkey.Models;
using System.Text.Json;

namespace BoxExpress.Infrastructure.External.SmartMonkey.Clients;

public class SmartMonkeyClient : IRoutePlanningClient
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;

    public SmartMonkeyClient(IConfiguration configuration, IMapper mapper)
    {
        _mapper = mapper;
        _httpClient = new HttpClient();
        _configuration = configuration;
    }

    public async Task<RoutingResponseCreatePlanDto> CreatePlanAsync(RoutingCreatePlanDto request)
    {
        string apiKey = _configuration.GetSection("SmartMoney:ApiKey").Value;
        string projectId = _configuration.GetSection("SmartMoney:ProjectId").Value;
        string baseUrl = _configuration.GetSection("SmartMoney:BaseUrl").Value;

        SmartMonkeyCreatePlanPayload payload = _mapper.Map<SmartMonkeyCreatePlanPayload>(request);

        var payloadString = JsonSerializer.Serialize(payload, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull
        });

        string requestUrl = $"{baseUrl}/plan?private_key={apiKey}&project_id={projectId}";
        HttpResponseMessage response = await _httpClient.PostAsync(requestUrl, new StringContent(payloadString, Encoding.UTF8, "application/json"));
        string responseContent = await response.Content.ReadAsStringAsync();
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error al crear el plan: {responseContent}");
        }
        SmartMonkeyCreatePlanResponse? responseModel = JsonSerializer.Deserialize<SmartMonkeyCreatePlanResponse>(responseContent);
        return _mapper.Map<RoutingResponseCreatePlanDto>(responseModel);
    }
}