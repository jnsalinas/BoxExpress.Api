using Microsoft.AspNetCore.Http;

namespace BoxExpress.Application.Dtos;

public class InventoryHoldResolutionDto
{
    public int InventoryHoldId { get; set; }
    public string? Notes { get; set; }
    public IFormFile? Photo { get; set; }
}

public class InventoryHoldMassiveResolutionDto
{
    public List<InventoryHoldResolutionDto> InventoryHoldResolutions { get; set; }
}