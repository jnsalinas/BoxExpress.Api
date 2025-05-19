using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Application.Interfaces;

public interface IInventoryHoldService
{
    Task<ApiResponse<IEnumerable<InventoryHoldDto>>> GetAllAsync(InventoryHoldFilterDto filter);
}
