using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExpress.Application.Dtos
{
    public class CreateOrderDto
    {
        public int Id { get; set; }
        public int? TimeSlotId { get; set; }
        public int StoreId { get; set; }
        public int? CreatedBy { get; set; }
        public int CreatorId { get; set; }
        public int OrderStatusId { get; set; }
        public int CategoryId { get; set; }
        public decimal? DeliveryFee { get; set; }
        public int CurrencyId { get; set; }
        public int WarehouseId { get; set; }
        public int ClientId { get; set; }
        public int ClientAddressId { get; set; }
        public int CityId { get; set; }
        public int CountryId { get; set; }
        public DateTime? DeliveryDate { get; set; }
        public DateTime? RescheduleDate { get; set; }

        public string? Country { get; set; }
        public string? City { get; set; }
        public string? Code { get; set; }
        public string? Contains { get; set; }
        public string? SecondManagement { get; set; }
        public string? CourierComment { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime? ScheduledDate { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string? Notes { get; set; }
        public required List<OrderItemDto> OrderItems { get; set; }

        public int WalletId { get; set; }
    }
}
