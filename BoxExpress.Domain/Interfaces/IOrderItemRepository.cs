
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IOrderItemRepository : IRepository<OrderItem>
{
    Task<List<OrderItem>> GetByOrderIdAsync(int orderId);
}