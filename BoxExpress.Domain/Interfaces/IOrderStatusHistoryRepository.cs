
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IOrderStatusHistoryRepository : IRepository<OrderStatusHistory>
{
    Task<List<OrderStatusHistory>> GetByOrderIdAsync(int orderId);
}