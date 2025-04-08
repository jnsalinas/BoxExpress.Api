// using BoxExpress.Domain.Entities;
// using BoxExpress.Domain.Interfaces;

// namespace BoxExpress.Infrastructure.Repositories
// {
//     public class InMemoryRepository<T> : IRepository<T> where T : BaseEntity
//     {
//         private readonly List<T> _data = new();
//         private int _nextId = 1;

//         public Task<T> AddAsync(T entity)
//         {
//             entity.Id = _nextId++;
//             _data.Add(entity);
//             return Task.FromResult(entity);
//         }

//         public Task<List<T>> GetAllAsync()
//         {
//             return Task.FromResult(_data.ToList());
//         }

//         public Task<T?> GetByIdAsync(int id)
//         {
//             var entity = _data.FirstOrDefault(e => e.Id == id);
//             return Task.FromResult(entity);
//         }

//         public Task<T> UpdateAsync(T entity)
//         {
//             var index = _data.FindIndex(e => e.Id == entity.Id);
//             if (index != -1)
//             {
//                 _data[index] = entity;
//             }
//             return Task.FromResult(entity);
//         }

//         public Task DeleteAsync(int id)
//         {
//             var entity = _data.FirstOrDefault(e => e.Id == id);
//             if (entity != null)
//             {
//                 _data.Remove(entity);
//             }
//             return Task.CompletedTask;
//         }
//     }
// }
