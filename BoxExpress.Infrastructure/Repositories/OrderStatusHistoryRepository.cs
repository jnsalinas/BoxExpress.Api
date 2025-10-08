using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Infrastructure.Repositories;

public class OrderStatusHistoryRepository : Repository<OrderStatusHistory>, IOrderStatusHistoryRepository
{
    private readonly BoxExpressDbContext _context;

    public OrderStatusHistoryRepository(BoxExpressDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<List<OrderStatusHistory>> GetByOrderIdAsync(int orderId)
    {
        return await _context.OrderStatusHistories
            .Where(w => w.OrderId.Equals(orderId))
            .Include(x => x.OldStatus)
            .Include(x => x.NewStatus)
            .Include(x => x.Creator)
            .Include(x => x.DeliveryProvider)
            .OrderByDescending(x => x.CreatedAt)
            .ToListAsync();
    }

    public async Task<List<OrderStatusCountResult>> GetOrderStatusCountByStatusesAsync(OrderStatusHistoryFilter filter)
    {
        var query = _context.OrderStatusHistories.AsQueryable();

        if (filter.NewStatusesId != null && filter.NewStatusesId.Any())
            query = query.Where(x => filter.NewStatusesId.Contains(x.NewStatusId));

        if (filter.OrderIds != null && filter.OrderIds.Any())
            query = query.Where(x => filter.OrderIds.Contains(x.OrderId));

        var result = await query
            .GroupBy(x => new { x.OrderId, x.NewStatusId })
            .Select(g => new OrderStatusCountResult
            {
                OrderId = g.Key.OrderId,
                NewStatusId = g.Key.NewStatusId,
                Notes = GetNotesText(g.OrderByDescending(x => x.CreatedAt).ToList()),
                Count = g.Count()
            })
            .ToListAsync();

        return result;
       
    }

    //todo mejor tal vez devolver toda la info en una lista al front y ahi mostrar la info como la queremos
    private static string? GetNotesText(List<OrderStatusHistory> notes)
    {
        string result = string.Empty;
        foreach (var note in notes)
        {
            if (!string.IsNullOrEmpty(note.Notes))
            {
                result += "• Fecha: " + note.CreatedAt.ToString("dd/MM/yyyy HH:mm") + "\n";
                result += "Mensajero: " + note.CourierName + "\n";
                result += "Motivo: " + note.Notes + "\n\n";
            }
        }
        if (string.IsNullOrEmpty(result))
        {
            return null;
        }
        return result;
    }

    public async Task<List<OrderStatusHistory>> GetFilteredAsync(OrderStatusHistoryFilter filter)
    {
        var query = _context.OrderStatusHistories
        .Include(x => x.DeliveryProvider)
        .AsQueryable();

        if (filter.OrderId.HasValue)
        {
            query = query.Where(x => x.OrderId == filter.OrderId.Value);
        }

        if (filter.OldStatusId.HasValue)
        {
            query = query.Where(x => x.OldStatusId == filter.OldStatusId.Value);
        }

        if (filter.NewStatusId.HasValue)
        {
            query = query.Where(x => x.NewStatusId == filter.NewStatusId.Value);
        }

        if (filter.NewStatusId.HasValue)
        {
            query = query.Where(x => x.NewStatusId == filter.NewStatusId.Value);
        }

        if (filter.NewStatusesId != null && filter.NewStatusesId.Any())
        {
            query = query.Where(x => filter.NewStatusesId.Contains(x.NewStatusId));
        }

        if (filter.OrderIds != null && filter.OrderIds.Any())
        {
            query = query.Where(x => filter.OrderIds.Contains(x.OrderId));
        }

        return query.ToList();
    }
}