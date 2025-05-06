using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IBankService
{
    Task<ApiResponse<IEnumerable<BankDto>>> GetAllAsync(BankFilterDto filter);
}
