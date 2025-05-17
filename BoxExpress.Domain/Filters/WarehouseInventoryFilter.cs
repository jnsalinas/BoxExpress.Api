namespace BoxExpress.Domain.Filters;

public class WarehouseInventoryFilter : BaseFilter
{

    public int WarehouseId { get; set; }
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public int ReservedQuantity { get; set; }
    public string? Query { get; set; }   
}