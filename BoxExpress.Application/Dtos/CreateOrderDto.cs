using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExpress.Application.Dtos
{
    public class CreateOrderDto
    {
        // Client Information
        public string ClientFirstName { get; set; } = string.Empty;
        public string ClientLastName { get; set; } = string.Empty;
        public int ClientDocumentTypeId { get; set; }
        public string ClientDocument { get; set; } = string.Empty;
        public string ClientEmail { get; set; } = string.Empty;
        public string? ClientExternalId { get; set; }

        // Address Information
        public string ClientAddress { get; set; } = string.Empty;
        public string? ClientAddressComplement { get; set; }
        public int CityId { get; set; }
        public string? Latitude { get; set; }
        public string? Longitude { get; set; }
        public string? ClientAddress2 { get; set; }

        // Order Information
        public int StoreId { get; set; }
        public decimal? DeliveryFee { get; set; }
        public int CurrencyId { get; set; }
        public string? Code { get; set; }
        public string? Contains { get; set; }
        public decimal TotalAmount { get; set; }
        public string? Notes { get; set; }
        public string? ExternalId { get; set; }

        // Order Items
        public required List<OrderItemDto> OrderItems { get; set; }
        public int? CreatorId { get; set; }
    }
}
