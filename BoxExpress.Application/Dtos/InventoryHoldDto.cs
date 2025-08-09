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
        public int? WarehouseInventoryTransferDetailId { get; set; }
        public int? WarehouseInventoryTransferId { get; set; }
        public int? ProductLoanDetailId { get; set; }
        public int? ProductLoanId { get; set; }
        public InventoryHoldType Type { get; set; }
        public InventoryHoldStatus Status { get; set; }
        public int CreatorId { get; set; }
        public string? ClientFullName { get; set; }
        public string? WarehouseName { get; set; }
        public int? ItemIndex { get; set; }

    }
}



