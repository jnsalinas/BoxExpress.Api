using Microsoft.AspNetCore.Http;

namespace BoxExpress.Application.Dtos
{
    public class ChangeStatusDto
    {
        public int OrderId { get; set; }
        public string? CourierName { get; set; }
        public int? DeliveryProviderId { get; set; }
        public IFormFile? Photo { get; set; }
    }
}
