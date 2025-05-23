using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Application.Interfaces;

public interface IInventoryMovementService
{
    Task<ApiResponse<bool>> ProcessDeliveryAsync(Order order);
    Task<ApiResponse<bool>> RevertDeliveryAsync(Order order);
     Task AdjustInventoryAsync(InventoryMovement movement, bool moveReserved = true, bool movePendingReturn = false);
    Task<ApiResponse<IEnumerable<InventoryMovementDto>>> GetAllAsync(InventoryMovementFilterDto filter);
}
