using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BoxExpress.Application.Dtos
{
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public DateTime Expiration { get; set; }
        public string? Role { get; set; }
        public string? StoreId { get; set; }
        public string? WarehouseName { get; set; }
        public string? CountryId { get; set; }
    }
}
