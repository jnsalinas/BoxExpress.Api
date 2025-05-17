using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IWarehouseInventoryService
{
    Task<ApiResponse<IEnumerable<ProductVariantDto>>> GetAllAsync(WarehouseInventoryFilterDto filter);
    Task<ApiResponse<List<ProductVariantAutocompleteDto>>> GetVariantsAutocompleteAsync(string query, int WarehouseOriginId);
    Task<ApiResponse<IEnumerable<ProductDto>>> GetWarehouseProductSummaryAsync(WarehouseInventoryFilterDto filter);
}