namespace BoxExpress.Domain.Enums;
public enum InventoryHoldStatus
{
    Active = 1,
    Released = 2, //La reserva se anuló, sin que se use el inventario., Se cancela la orden,. Se rechaza la solicitud de transferencia.
    Consumed = 3
}