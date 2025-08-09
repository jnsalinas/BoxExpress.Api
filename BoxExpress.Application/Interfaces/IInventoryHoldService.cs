using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Interfaces;

public interface IInventoryHoldService
{
    Task<ApiResponse<bool>> HoldInventoryForOrderAsync(int warehouseId, List<OrderItem> orderItems, InventoryHoldStatus status);
    Task<ApiResponse<IEnumerable<InventoryHoldDto>>> GetAllAsync(InventoryHoldFilterDto filter);
    Task<ApiResponse<bool>> AcceptReturnAsync(InventoryHoldResolutionDto dto);
    Task<ApiResponse<bool>> RejectReturnAsync(InventoryHoldResolutionDto dto);
    Task<ApiResponse<bool>> CreateInventoryHoldAsync(
    WarehouseInventory warehouseInventories,
    int quantity,
    InventoryHoldType holdType,
    InventoryHoldStatus holdStatus,
    int? orderItemId = null,
    int? warehouseInventoryTransferDetailId = null,
    int? productLoanDetailId = null);
}
