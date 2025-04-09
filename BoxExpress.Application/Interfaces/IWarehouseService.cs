using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;

namespace BoxExpress.Application.Interfaces;

public interface IWarehouseService
{
    Task<IEnumerable<WarehouseDto>> GetAllAsync(WarehouseFilterDto filter);
}
