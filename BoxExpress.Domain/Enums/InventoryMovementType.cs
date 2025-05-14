namespace BoxExpress.Domain.Enums;
public enum InventoryMovementType
{
    OrderDelivered = 1,
    OrderCanceled = 2,
    TransferSent = 3,
    TransferReceived = 4,
    ManualAdjustment = 5,
    InitialLoad = 6,
    OrderDeliveryReverted = 7,
    InitialStock = 8,
}


