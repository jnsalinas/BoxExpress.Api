
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IOrderRepository : IRepository<Order>
{
    Task<List<Order>> GetFilteredAsync(OrderFilter filter);
    Task<Order?> GetByIdWithDetailsAsync(int id);
}