using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Entities;

namespace BoxExpress.Infrastructure.Persistence
{
    //Correr migrations: 
    // dotnet ef migrations add InitialCreate --project BoxExpress.Infrastructure --startup-project BoxExpress.Api
    //dotnet ef database update --project BoxExpress.Infrastructure --startup-project BoxExpress.Api

    public class BoxExpressDbContext : DbContext
    {
        public BoxExpressDbContext(DbContextOptions<BoxExpressDbContext> options) : base(options) { }

        public DbSet<Order> Orders { get; set; }
        // Agrega aquí otros DbSet según los modelos que tengas

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Puedes añadir configuraciones personalizadas aquí si quieres
        }
    }
}
