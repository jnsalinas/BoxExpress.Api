using BoxExpress.Domain.Entities;

namespace BoxExpress.Domain.Interfaces;

public interface IProductLoanDetailRepository : IRepository<ProductLoanDetail>
{
    Task<IEnumerable<ProductLoanDetail>> GetByProductLoanIdAsync(int productLoanId);
} 