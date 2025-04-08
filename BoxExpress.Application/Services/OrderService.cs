using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services;

public class OrderService : IOrderService
{
    private readonly IRepository<Order> _repository;

    public OrderService(IRepository<Order> repository)
    {
        _repository = repository;
    }

    public async Task<IEnumerable<Order>> GetAllAsync()
        => await _repository.GetAllAsync();

    public async Task<Order?> GetByIdAsync(int id)
    {
        return await _repository.GetByIdAsync(id);
    }

    public async Task<Order> AddAsync(Order order)
    {
        order.CreatedAt = DateTime.UtcNow;
        return await _repository.AddAsync(order);
    }
}
