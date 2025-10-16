namespace BoxExpress.Domain.Entities;

using System.ComponentModel.DataAnnotations.Schema;
public class OrderItem : BaseEntity
{
    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public string? ExternalId { get; set; }
    
    [NotMapped]
    public string? FullName => $"{ProductVariant.Product.Name} {ProductVariant.Name} unidades: {Quantity}";
}