using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class DeliveryProviderRepository : Repository<DeliveryProvider>, IDeliveryProviderRepository
{
    private readonly BoxExpressDbContext _context;

    public DeliveryProviderRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }
}