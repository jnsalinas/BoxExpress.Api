using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BoxExpressDbContext _context;

    public IProductRepository Products { get; }
    public IProductVariantRepository Variants { get; }
    public IWarehouseInventoryRepository Inventories { get; }

    public UnitOfWork(
        BoxExpressDbContext context,
        IProductRepository products,
        IProductVariantRepository variants,
        IWarehouseInventoryRepository inventories)
    {
        _context = context;
        Products = products;
        Variants = variants;
        Inventories = inventories;
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
