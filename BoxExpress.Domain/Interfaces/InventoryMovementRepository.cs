
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;

public interface IInventoryMovementRepository : IRepository<InventoryMovement>
{
    Task<(List<InventoryMovement> InventoryMovements, int TotalCount)> GetFilteredAsync(InventoryMovementFilter filter);
}