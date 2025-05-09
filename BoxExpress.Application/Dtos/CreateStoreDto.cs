using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExpress.Application.Dtos
{
    public class CreateStoreDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Phone { get; set; }
        public int CountryId { get; set; }
        public int CityId { get; set; }
        public string? PickupAddress { get; set; }
        public string? LegalName { get; set; }
        public string? DocumentNumber { get; set; }

        public string StoreName { get; set; } = string.Empty;
        public decimal Balance { get; set; }
    }
}
