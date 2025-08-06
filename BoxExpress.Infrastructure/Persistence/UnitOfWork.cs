using BoxExpress.Domain.Interfaces;
using Microsoft.EntityFrameworkCore.Storage;

namespace BoxExpress.Infrastructure.Persistence;

public class UnitOfWork : IUnitOfWork
{
    private readonly BoxExpressDbContext _context;
    private IDbContextTransaction? _transaction;
    public bool HasActiveTransaction => _transaction != null;
    public IProductRepository Products { get; }
    public IProductVariantRepository Variants { get; }
    public IWarehouseInventoryRepository Inventories { get; }
    public IWarehouseInventoryTransferRepository WarehouseInventoryTransfers { get; }
    public IWalletRepository Wallets { get; }
    public IStoreRepository Stores { get; }
    public IUserRepository Users { get; }
    public IInventoryMovementRepository InventoryMovements { get; }
    public IInventoryHoldRepository InventoryHolds { get; }
    public IOrderRepository Orders { get; }
    public IOrderItemRepository OrderItems { get; }
    public IOrderCategoryHistoryRepository OrderCategoryHistories { get; }
    public IOrderStatusHistoryRepository OrderStatusHistories { get; }
    public IWalletTransactionRepository WalletTransactions { get; }
    public IClientRepository Clients { get; }
    public IClientAddressRepository ClientAddresses { get; }
    public IWarehouseRepository Warehouses { get; }
    public IProductLoanRepository ProductLoans { get; }
    public IProductLoanDetailRepository ProductLoanDetails { get; }
    public UnitOfWork(
        BoxExpressDbContext context,
        IProductRepository products,
        IProductVariantRepository variants,
        IWarehouseInventoryRepository inventories,
        IWarehouseInventoryTransferRepository transfers,
        IInventoryMovementRepository movements,
        IWalletRepository wallets,
        IStoreRepository stores,
        IInventoryHoldRepository holds,
        IUserRepository users,
        IOrderRepository orders,
        IOrderItemRepository orderItems,
        IOrderCategoryHistoryRepository orderCategoryHistories,
        IOrderStatusHistoryRepository orderStatusHistories,
        IWalletTransactionRepository walletTransactions,
        IClientRepository clients,
        IClientAddressRepository clientAddresses,
        IWarehouseRepository warehouses,
        IProductLoanRepository productLoans,
        IProductLoanDetailRepository productLoanDetails)
    {
        _context = context;
        Products = products;
        Variants = variants;
        Inventories = inventories;
        InventoryHolds = holds;
        InventoryMovements = movements;
        WarehouseInventoryTransfers = transfers;
        Wallets = wallets;
        Stores = stores;
        Users = users;
        Orders = orders;
        OrderItems = orderItems;
        OrderCategoryHistories = orderCategoryHistories;
        OrderStatusHistories = orderStatusHistories;
        WalletTransactions = walletTransactions;
        Clients = clients;
        ClientAddresses = clientAddresses;
        Warehouses = warehouses;
        ProductLoans = productLoans;
        ProductLoanDetails = productLoanDetails;
        _transaction = null;
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
