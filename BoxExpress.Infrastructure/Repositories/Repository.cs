using Microsoft.EntityFrameworkCore;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Infrastructure.Persistence;

namespace BoxExpress.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly BoxExpressDbContext _context;

    public Repository(BoxExpressDbContext context)
    {
        _context = context;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _context.Set<T>().AddAsync(entity);
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task<List<T>> GetAllAsync()
    {
        return await _context.Set<T>().ToListAsync();
    }

    public Task<T?> GetByIdAsync(int id)
    {
        return _context.Set<T>().FirstOrDefaultAsync(x => x.Id.Equals(id));
    }
    public async Task<T> UpdateAsync(T entity)
    {
        // Limpiar el contexto antes de hacer el update para evitar conflictos de tracking
        _context.ChangeTracker.Clear();
        
        var entry = _context.Attach(entity);
        entry.State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return entity;
    }

    public async Task DeleteAsync(int id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }
    }
}
