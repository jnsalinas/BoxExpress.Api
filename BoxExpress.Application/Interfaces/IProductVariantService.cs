using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IProductVariantService
{
    Task<ApiResponse<List<ProductVariantDto>>> GetAllAsync(ProductVariantFilterDto filter);
    Task<ApiResponse<ProductVariantDto?>> GetByIdAsync(int id);
    Task<ApiResponse<List<ProductVariantDto?>>> GetByNameAsync(string name, int storeId);
    Task<ApiResponse<List<ProductVariantDto>>> GetVariantsAutocompleteAsync(string query);
}