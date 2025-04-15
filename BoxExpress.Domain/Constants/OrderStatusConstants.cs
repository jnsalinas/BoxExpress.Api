namespace BoxExpress.Domain.Constants;

public static class OrderStatusConstants
{
    public const string Unscheduled = "Sin programar";
    public const string Scheduled = "Programado";
     public const string OnTheWay = "En ruta"; //todo revisar si se va a usar seria buena opcion
    public const string Delivered = "Entregado";
    public const string Cancelled = "Cancelado";
    public const string CancelledAlt = "Cancelado 1";
}

//OrderCategory