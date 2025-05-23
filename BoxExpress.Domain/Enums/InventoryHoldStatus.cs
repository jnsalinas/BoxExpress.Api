namespace BoxExpress.Domain.Enums;
public enum InventoryHoldStatus
{
    Active = 1,       // Retención activa: inventario reservado
    Released = 2,     // No se usó inventario, se liberó la retención
    Consumed = 3,     // Ya se consumió (ej: se envió producto)
    PendingReturn = 4, // Esperando confirmación de devolución (nuevo)
    Returned = 5,     // Producto volvió a bodega
    NotReturned = 6   // Producto NO volvió (se pierde o daña)
}
