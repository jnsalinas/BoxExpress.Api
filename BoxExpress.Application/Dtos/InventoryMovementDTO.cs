using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Dtos
{
    public class InventoryMovementDto: BaseDto
    {
        public WarehouseDto Warehouse { get; set; }
        public int WarehouseId { get; set; }
        public int ProductVariantId { get; set; }
        public InventoryMovementType MovementType { get; set; }
        public int Quantity { get; set; }
        public int? OrderId { get; set; }
        public int? TransferId { get; set; }
        public string? Reference { get; set; }
        public string? Notes { get; set; }
    }
}



