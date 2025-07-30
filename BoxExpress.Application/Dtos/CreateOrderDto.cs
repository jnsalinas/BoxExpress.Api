using System.Collections.Generic;

namespace BoxExpress.Application.Dtos
{
    public class CreateOrderDto
    {
        // Client Information
        public string ClientFirstName { get; set; } = string.Empty;
        public string ClientLastName { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string ClientPhone { get; set; } = string.Empty;

        // Address Information
        public string ClientAddress { get; set; } = string.Empty;
        public string? ClientAddressComplement { get; set; }
        public int CityId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public string? PostalCode { get; set; }

        // Order Information
        public int StoreId { get; set; }
        public decimal? DeliveryFee { get; set; }
        public int CurrencyId { get; set; }
        public string? Code { get; set; }
        public decimal TotalAmount { get; set; }

        public string? Notes { get; set; }

        // Order Items
        public required List<OrderItemDto> OrderItems { get; set; }
    }
}
