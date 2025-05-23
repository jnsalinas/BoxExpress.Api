using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Dtos
{
    public class InventoryHoldDto : BaseDto
    {
        public int WarehouseInventoryId { get; set; }
        public WarehouseInventoryDto WarehouseInventory { get; set; } = null!;
        public int Quantity { get; set; }
        public int? OrderItemId { get; set; }
        public OrderItemDto? OrderItem { get; set; }
        // public OrderItem? OrderItem { get; set; }
        public int? TransferId { get; set; }
        // public WarehouseInventoryTransfer? Transfer { get; set; } = null!;
        public InventoryHoldType Type { get; set; }
        public InventoryHoldStatus Status { get; set; }
        public int CreatorId { get; set; }
        // public User Creator { get; set; } = null!;
    }
}



