
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;

public interface IProductVariantRepository : IRepository<ProductVariant>
{
    Task<List<ProductVariant?>> GetByVariantNameAndStoreId(string productVariantName, int storeId);
    Task<List<ProductVariant>> GetAllAsync(ProductVariantFilter filter);
    Task<ProductVariant?> GetByIdWithDetailsAsync(int id);
    Task<List<ProductVariant>> GetByIdsAsync(List<int> ids);
    Task<ProductVariant?> GetByProductNameVariantNameAndStoreId(string productName, string productVariantName, int storeId);
}