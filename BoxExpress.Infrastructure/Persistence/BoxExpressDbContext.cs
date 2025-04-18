using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Infrastructure.Persistence
{
    //Correr migrations: 
    //dotnet ef migrations add FixWallet --project BoxExpress.Infrastructure --startup-project BoxExpress.Api
    //dotnet ef database update --project BoxExpress.Infrastructure --startup-project BoxExpress.Api

    public class BoxExpressDbContext : DbContext
    {
        public BoxExpressDbContext(DbContextOptions<BoxExpressDbContext> options) : base(options) { }

        public DbSet<Country> Countries { get; set; }
        public DbSet<City> Cities { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Store> Stores { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Warehouse> Warehouses { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ProductVariant> ProductVariants { get; set; }
        public DbSet<WarehouseInventory> WarehouseInventories { get; set; }
        public DbSet<OrderStatus> OrderStatuses { get; set; }
        public DbSet<OrderCategory> OrderCategories { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderItem> OrderItems { get; set; }
        public DbSet<OrderStatusHistory> OrderStatusHistories { get; set; }
        public DbSet<OrderCategoryHistory> OrderCategoryHistories { get; set; }
        public DbSet<TransactionType> TransactionTypes { get; set; }
        public DbSet<Wallet> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }
        public DbSet<WithdrawalRequest> WithdrawalRequests { get; set; }
        public DbSet<TimeSlot> TimeSlots { get; set; }
        public DbSet<WarehouseInventoryTransfer> WarehouseInventoryTransfers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Aplica DeleteBehavior.Restrict a todas las foreign keys
            foreach (var entity in modelBuilder.Model.GetEntityTypes())
            {
                foreach (var fk in entity.GetForeignKeys())
                {
                    fk.DeleteBehavior = DeleteBehavior.Restrict;
                }
            }

            base.OnModelCreating(modelBuilder);
        }
    }
}
