using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IOrderCategoryService
{
    Task<ApiResponse<IEnumerable<OrderCategoryDto>>> GetAllAsync(OrderCategoryFilterDto filter);

}
