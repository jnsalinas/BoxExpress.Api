using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IProductVariantService
{
    Task<ApiResponse<ProductVariantDto?>> GetByIdAsync(int id);
    // Task<ApiResponse<List<ProductVariantAutocompleteDto>>> GetVariantsAutocompleteAsync(string query, int WarehouseOriginId);
}