using BoxExpress.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace BoxExpress.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BoxExpressDbContext _context;
    private IDbContextTransaction? _transaction;
    public IProductRepository Products { get; }
    public IProductVariantRepository Variants { get; }
    public IWarehouseInventoryRepository Inventories { get; }
    public IWarehouseInventoryTransferRepository WarehouseInventoryTransfers { get; }
    public IWalletRepository Wallets { get; }
    public IStoreRepository Stores { get; }
    public IUserRepository Users { get; }
    public IInventoryMovementRepository InventoryMovements { get; }

    public UnitOfWork(
        BoxExpressDbContext context,
        IProductRepository products,
        IProductVariantRepository variants,
        IWarehouseInventoryRepository inventories,
        IWarehouseInventoryTransferRepository transfers,
        IInventoryMovementRepository movements,
        IWalletRepository wallets,
        IStoreRepository stores,
        IUserRepository users)
    {
        _context = context;
        Products = products;
        Variants = variants;
        Inventories = inventories;
        InventoryMovements = movements;
        WarehouseInventoryTransfers = transfers;
        Wallets = wallets;
        Stores = stores;
        Users = users;
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            _transaction = null;
        }
    }

    public async Task RollbackAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            _transaction = null;
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
