
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IOrderStatusRepository : IRepository<OrderStatus>
{
    Task<OrderStatus?> GetByNameAsync(string name);
}