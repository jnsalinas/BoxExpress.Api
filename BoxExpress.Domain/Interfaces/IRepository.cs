using BoxExpress.Domain.Entities;

namespace BoxExpress.Domain.Interfaces;
public interface IRepository<T> where T : BaseEntity
{
    Task<T> AddAsync(T entity);
    Task<List<T>> GetAllAsync();
    Task<T?> GetByIdAsync(int id);
    Task<T> UpdateAsync(T entity);
    Task DeleteAsync(int id);
}
