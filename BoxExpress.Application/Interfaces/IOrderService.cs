using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Application.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto filter);
    Task<ApiResponse<OrderDetailDto?>> GetByIdAsync(int id);
    Task<ApiResponse<OrderDto>> UpdateWarehouseAsync(int orderId, int warehouseId);
}
