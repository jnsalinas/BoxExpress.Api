
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;

public interface IProductVariantRepository : IRepository<ProductVariant>
{
    Task<ProductVariant?> GetByIdWithDetailsAsync(int id);
    Task<List<ProductVariant>> GetBySkusAsync(HashSet<string> skus);
    // Task<List<ProductVariant>> GetVariantsAutocompleteAsync(string query, int warehouseOrigonId);
}