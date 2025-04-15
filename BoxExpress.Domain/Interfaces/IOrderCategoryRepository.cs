
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IOrderCategoryRepository : IRepository<OrderCategory>
{
    Task<OrderCategory?> GetByNameAsync(string name);
}