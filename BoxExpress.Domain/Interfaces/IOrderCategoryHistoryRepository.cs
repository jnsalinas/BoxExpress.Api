
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;
public interface IOrderCategoryHistoryRepository : IRepository<OrderCategoryHistory>
{
    Task<List<OrderCategoryHistory>> GetByOrderIdAsync(int orderId);
}