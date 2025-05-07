using BoxExpress.Domain.Enums;

namespace BoxExpress.Domain.Extensions;
public static class InventoryTransferStatusExtensions
{
    public static string ToSpanish(this InventoryTransferStatus status)
    {
        return status switch
        {
            InventoryTransferStatus.Pending => "Pendiente",
            InventoryTransferStatus.Accepted => "Aceptada",
            InventoryTransferStatus.Rejected => "Rechazada",
            _ => status.ToString()
        };
    }
}
