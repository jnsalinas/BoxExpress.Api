using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Application.Dtos
{
    public class ClientDto
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Document { get; set; } = string.Empty;
        public string Phone { get; set; } = string.Empty;
        public string? ExternalId { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? Email { get; set; }
        public ICollection<ClientAddressDto> Addresses { get; set; } = new List<ClientAddressDto>();
    }
}
