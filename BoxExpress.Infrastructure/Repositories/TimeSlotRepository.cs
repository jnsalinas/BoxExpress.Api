using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class TimeSlotRepository : Repository<TimeSlot>, ITimeSlotRepository
{
    private readonly BoxExpressDbContext _context;

    public TimeSlotRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }
}