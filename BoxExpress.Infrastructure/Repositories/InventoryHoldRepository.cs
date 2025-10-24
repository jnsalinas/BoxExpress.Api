using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Infrastructure.Repositories;

public class InventoryHoldRepository : Repository<InventoryHold>, IInventoryHoldRepository
{
    private readonly BoxExpressDbContext _context;

    public InventoryHoldRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<InventoryHold?> GetByIdWithDetailsAsync(int id)
    {
        return await _context.InventoryHolds
            .Include(x => x.WarehouseInventory)
            .Include(x => x.OrderItem)
            .Where(w => w.Id == id)
            .FirstOrDefaultAsync();
    }

    public async Task<List<InventoryHold>> GetByWarehouseInventoryAndStatus(int warehouseinvetoryId, InventoryHoldStatus? status)
    {
        return await _context.InventoryHolds.Include(x => x.WarehouseInventory)
               .Where(w => w.WarehouseInventoryId == warehouseinvetoryId
               && (!status.HasValue || (w.Status == status)))
               .ToListAsync();
    }

    public async Task<(List<InventoryHold> InventoryHolds, int TotalCount)> GetFilteredAsync(InventoryHoldFilter filter)
    {
        var query = _context.InventoryHolds
            .Include(w => w.Creator)
            .Include(w => w.OrderItem)
                .ThenInclude(w => w.Order)
                    .ThenInclude(w => w.Client)
            .Include(w => w.OrderItem)
                .ThenInclude(w => w.Order)
                    .ThenInclude(w => w.Store)
            .Include(w => w.WarehouseInventory)
                .ThenInclude(wi => wi.ProductVariant)
                    .ThenInclude(pv => pv.Product)
            .Include(w => w.WarehouseInventory)
                .ThenInclude(wi => wi.Warehouse)
            .Include(w => w.WarehouseInventoryTransferDetail)
            .Include(w => w.ProductLoanDetail)
            .Include(w => w.OrderStatusHistory)
                .ThenInclude(w => w.DeliveryProvider)
            .AsQueryable();

        if (filter.WarehouseInventoryId.HasValue)
        {
            query = query.Where(w => w.WarehouseInventoryId == filter.WarehouseInventoryId.Value);
        }

        if (filter.Status.HasValue)
        {
            query = query.Where(w => w.Status == filter.Status.Value);
        }

        if (filter.Statuses != null && filter.Statuses.Any())
        {
            query = query.Where(w => filter.Statuses.Contains(w.Status));
        }

        if (filter.EndDate.HasValue)
        {
            query = query.Where(w => w.ResolvedAt <= filter.EndDate.Value);
        }

        if (filter.StartDate.HasValue)
        {
            query = query.Where(w => w.ResolvedAt >= filter.StartDate.Value);
        }

        if (filter.OrderId.HasValue)
        {
            query = query.Where(w => w.OrderItem != null && w.OrderItem.OrderId == filter.OrderId.Value);
        }

        if (filter.WarehouseId.HasValue)
        {
            query = query.Where(w => w.WarehouseInventory != null && w.WarehouseInventory.WarehouseId == filter.WarehouseId.Value);
        }

        if (filter.ProductLoanDetailId.HasValue)
        {
            query = query.Where(w => w.ProductLoanDetailId == filter.ProductLoanDetailId.Value);
        }

        if (filter.ProductLoanId.HasValue)
        {
            query = query.Where(w => w.ProductLoanDetail != null && w.ProductLoanDetail.ProductLoanId == filter.ProductLoanId.Value);
        }

        if (filter.CreatedAt.HasValue)
        {
            query = query.Where(w => w.CreatedAt >= filter.CreatedAt);
        }

        if (filter.EndCreatedAt.HasValue)
        {
            query = query.Where(w => w.CreatedAt <= filter.EndCreatedAt);
        }

        if (filter.DeliveryProviderId.HasValue)
        {
            query = query.Where(w => w.OrderStatusHistory != null && w.OrderStatusHistory.DeliveryProviderId == filter.DeliveryProviderId.Value);
        }

        if (filter.StoreId.HasValue)
        {
            query = query.Where(w => w.OrderItem != null && w.OrderItem.Order.StoreId == filter.StoreId.Value);
        }

        if (!string.IsNullOrEmpty(filter.Query))
        {
            query = query.Where(w =>
            w.OrderItem != null
                && (w.OrderItem.Order.Client.FirstName + " " + w.OrderItem.Order.Client.LastName).Contains(filter.Query)
                || w.OrderItem.Order.Client.Phone.Contains(filter.Query)
                || w.OrderItem.Order.Client.Document.Contains(filter.Query)
                || (w.OrderStatusHistory != null && !string.IsNullOrEmpty(w.OrderStatusHistory.CourierName) && w.OrderStatusHistory.CourierName.Contains(filter.Query))
            );
        }

        var total = query.Count();
        if (!filter.IsAll)
        {
            query = query
                .Skip((filter.Page - 1) * filter.PageSize)
                .Take(filter.PageSize);
        }

        return (await query.ToListAsync(), total);
    }

    public async Task<List<InventoryHold>> GetByOrderItemIdsAndStatus(List<int> listOrderItemIds, InventoryHoldStatus? status)
    {
        return await _context.InventoryHolds.Include(x => x.WarehouseInventory)
                .Where(w => w.OrderItemId.HasValue && listOrderItemIds.Contains(w.OrderItemId.Value)
                && (!status.HasValue || (w.Status == status)))
                .ToListAsync();
    }

    public async Task<List<InventoryHold>> GetByTransferIdsAndStatus(int transferId, InventoryHoldStatus? status)
    {
        return await _context.InventoryHolds
                .Where(w => w.WarehouseInventoryTransferDetailId.HasValue && w.WarehouseInventoryTransferDetail.WarehouseInventoryTransferId == transferId && (!status.HasValue || (w.Status == status)))
                .ToListAsync();
    }

}