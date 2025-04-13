using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IOrderStatusService
{
    Task<ApiResponse<IEnumerable<OrderStatusDto>>> GetAllAsync(OrderStatusFilterDto filter);

}
