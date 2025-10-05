
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;

public interface IOrderStatusHistoryRepository : IRepository<OrderStatusHistory>
{
    Task<List<OrderStatusHistory>> GetByOrderIdAsync(int orderId);
    Task<List<OrderStatusHistory>> GetFilteredAsync(OrderStatusHistoryFilter filter);
    Task<List<OrderStatusCountResult>> GetOrderStatusCountByStatusesAsync(OrderStatusHistoryFilter filter);
}