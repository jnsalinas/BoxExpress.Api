
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IProductVariantRepository : IRepository<ProductVariant>
{
    Task<List<ProductVariant>> GetVariantsAutocompleteAsync(string query);
}