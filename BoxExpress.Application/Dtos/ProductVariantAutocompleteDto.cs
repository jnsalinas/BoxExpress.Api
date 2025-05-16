namespace BoxExpress.Application.Dtos;

public class ProductVariantAutocompleteDto
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? Sku { get; set; }
    public string? ProductName { get; set; }
    public int? AvailableUnits { get; set; }
    public int ReservedQuantity { get; set; }
    public int Quantity { get; set; }
}