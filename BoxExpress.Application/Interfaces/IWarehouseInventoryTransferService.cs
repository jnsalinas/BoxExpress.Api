using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Interfaces;

public interface    IWarehouseInventoryTransferService
{
    Task<ApiResponse<IEnumerable<WarehouseInventoryTransferDto>>> GetAllAsync(WarehouseInventoryTransferFilterDto filter);
    Task<ApiResponse<WarehouseInventoryTransferDto?>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> CreateTransferAsync(WarehouseInventoryTransferDto warehouseInventoryTransferDto);
    Task<ApiResponse<bool>> AcceptTransferAsync(int transferId, int userId);
    Task<ApiResponse<bool>> RejectTransferAsync(int transferId, int userId, string rejectionReason);
    // Task<ApiResponse<bool>> HoldInventoryForOrderAsync(int warehouseId, List<OrderItem> orderItems, InventoryHoldStatus status);
}
