using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExpress.Application.Dtos
{
    public class ClientAddressDto
    {
        public int Id { get; set; }
        public int ClientId { get; set; }
        public string Address { get; set; } = string.Empty;
        public string? Address2 { get; set; } = string.Empty;
        public string? Complement { get; set; }
        public int CityId { get; set; }
        public decimal? Latitude { get; set; }
        public decimal? Longitude { get; set; }
        public bool IsDefault { get; set; } = false;
        public string? PostalCode { get; set; }
    }
}
