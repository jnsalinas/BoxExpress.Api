using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Enums;
namespace BoxExpress.Domain.Entities;
public class InventoryMovement : BaseEntity
{
    public int WarehouseId { get; set; }
    public Warehouse Warehouse { get; set; } = null!;

    public int ProductVariantId { get; set; }
    public ProductVariant ProductVariant { get; set; } = null!;

    public InventoryMovementType MovementType { get; set; } // enum: OrderDelivered, TransferSent, Manual, etc.

    public int Quantity { get; set; } // + para ingreso, - para salida

    // Relaciones opcionales según origen
    public int? OrderId { get; set; }
    public Order? Order { get; set; }

    public int? TransferId { get; set; }
    public WarehouseInventoryTransfer? Transfer { get; set; }

    // Descripción auxiliar
    public string? Reference { get; set; } // Código externo o texto
    public string? Notes { get; set; }

}
