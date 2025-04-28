
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IOrderRepository : IRepository<Order>
{
    Task<(List<Order> Transactions, int TotalCount)> GetFilteredAsync(OrderFilter filter);
    Task<Order?> GetByIdWithDetailsAsync(int id);
}