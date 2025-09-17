using System.Drawing;

namespace BoxExpress.Domain.Entities;

public class Order : BaseEntity
{
    public int? TimeSlotId { get; set; } // Franja horaria elegida
    public TimeSlot? TimeSlot { get; set; }
    public int StoreId { get; set; }
    public Store Store { get; set; } = null!;

    public int? CreatorId { get; set; }
    public User? Creator { get; set; }

    public int OrderStatusId { get; set; }
    public OrderStatus Status { get; set; } = null!;

    public int? OrderCategoryId { get; set; }
    public OrderCategory? Category { get; set; } = null!;

    public decimal DeliveryFee { get; set; }

    public int CurrencyId { get; set; }
    public Currency Currency { get; set; } = null!;

    public int? WarehouseId { get; set; }
    public Warehouse? Warehouse { get; set; }

    public int ClientId { get; set; }
    public Client Client { get; set; } = null!;
    public int ClientAddressId { get; set; }
    public ClientAddress ClientAddress { get; set; } = null!;
    public int? CityId { get; set; }
    public City? City { get; set; }
    public int? CountryId { get; set; }
    public Country? Country { get; set; }
    public DateTime? DeliveredDate { get; set; } // cuando realmente se entregó el pedido
    public DateTime? RescheduledDate { get; set; } // fecha de progrmaacion
    public string? Code { get; set; }
    public string? Contains { get; set; } // de shopify el resto debe sacar los productos que contiene
    public string? SecondManagement { get; set; }
    public string? CourierComment { get; set; }

    public decimal TotalAmount { get; set; }
    public DateTime? ScheduledDate { get; set; } // Fecha elegida para entrega
    public string? Notes { get; set; }
    public List<OrderItem> OrderItems { get; set; } = new();
    public string? ExternalId { get; set; }
    public bool? IsEnabled { get; set; }
}
