using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IWithdrawalRequestService
{
    Task<ApiResponse<IEnumerable<WithdrawalRequestDto>>>  GetAllAsync(WithdrawalRequestFilterDto filter);
    Task<ApiResponse<bool>> AddAsync(WithdrawalRequestCreateDto dto);
    Task<ApiResponse<bool>> ApproveAsync(int id, int userId, WithdrawalRequestApproveDto dto);
    Task<ApiResponse<bool>> RejectAsync(int id, int userId, WithdrawalRequestRejectDto dto);

}