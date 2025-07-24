using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Application.Interfaces;

public interface IOrderService
{
    Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto filter);
    Task<ApiResponse<OrderDto?>> GetByIdAsync(int id);
    Task<ApiResponse<OrderDto>> UpdateWarehouseAsync(int orderId, int warehouseId);
    Task<ApiResponse<OrderDto>> UpdateStatusAsync(int orderId, int warehouseId);
    Task<ApiResponse<OrderDto>> UpdateScheduleAsync(int orderId, OrderScheduleUpdateDto orderScheduleUpdateDto);
    Task<ApiResponse<List<OrderStatusHistoryDto>>> GetStatusHistoryAsync(int orderId);
    Task<ApiResponse<List<OrderCategoryHistoryDto>>> GetCategoryHistoryAsync(int orderId);
    Task<ApiResponse<List<OrderItemDto>>> GetProductsAsync(int orderId);
    Task<ApiResponse<IEnumerable<OrderSummaryDto>>> GetSummaryAsync(OrderFilterDto filter);
    Task<ApiResponse<OrderDto>> AddOrderAsync(CreateOrderDto createOrderDto);
    Task<ApiResponse<List<OrderExcelUploadResponseDto>>> AddOrdersFromExcelAsync(OrderExcelUploadDto dto);
}
