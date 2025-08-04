using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Domain.Interfaces;

public interface IProductLoanRepository : IRepository<ProductLoan>
{
    Task<IEnumerable<ProductLoan>> GetFilteredAsync(ProductLoanFilter filter);
    Task<ProductLoan?> GetByIdWithDetailsAsync(int id);
    Task<int> GetTotalCountAsync(ProductLoanFilter filter);
} 