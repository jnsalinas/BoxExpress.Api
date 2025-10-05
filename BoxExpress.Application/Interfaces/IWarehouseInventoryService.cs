using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IWarehouseInventoryService
{
    Task<ApiResponse<IEnumerable<ProductVariantDto>>> GetAllAsync(WarehouseInventoryFilterDto filter);
    Task<ApiResponse<List<ProductVariantAutocompleteDto>>> GetVariantsAutocompleteAsync(string query, int WarehouseOriginId);
    Task<ApiResponse<IEnumerable<ProductDto>>> GetWarehouseProductSummaryAsync(WarehouseInventoryFilterDto filter);
    Task<ApiResponse<WarehouseInventoryDto?>> GetByIdAsync(int id);
    Task<ApiResponse<WarehouseInventoryDto?>> UpdateAsync(int id, UpdateWarehouseInventoryDto dto);
    Task<ApiResponse<IEnumerable<ProductDto>>> GetWarehouseProductSummaryGroupAsync(WarehouseInventoryFilterDto filter);
    Task<ApiResponse<bool>> ManageOnTheWayInventoryAsync(int warehouseId, List<OrderItem> orderItems);
}