
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;

public interface IOrderRepository : IRepository<Order>
{
    Task<(List<Order> Orders, int TotalCount)> GetFilteredAsync(OrderFilter filter);
    Task<Order?> GetByIdWithDetailsAsync(int id);
    Task<List<OrderSummary>> GetSummaryAsync(OrderFilter filter);
    Task<Order?> GetByCodeAsync(string code, int storeId);
    Task<List<OrderSummary>> GetSummaryCategoryAsync(OrderFilter filter);
}