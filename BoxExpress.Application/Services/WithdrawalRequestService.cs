using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Services;

public class WithdrawalRequestService : IWithdrawalRequestService
{
    private readonly IWithdrawalRequestRepository _repository;
    private readonly IMapper _mapper;
    private readonly IWalletTransactionService _walletTransactionService;

    public WithdrawalRequestService(
        IWithdrawalRequestRepository repository,
        IMapper mapper,
        IWalletTransactionService walletTransactionService)
    {
        _walletTransactionService = walletTransactionService;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<WithdrawalRequestDto>>> GetAllAsync(WithdrawalRequestFilterDto filter)
    {
        var (withdrawalRequest, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<WithdrawalRequestFilter>(filter));
        return ApiResponse<IEnumerable<WithdrawalRequestDto>>.Success(_mapper.Map<List<WithdrawalRequestDto>>(withdrawalRequest), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<bool>> AddAsync(WithdrawalRequestCreateDto dto)
    {
        WithdrawalRequest withdrawalRequest = _mapper.Map<WithdrawalRequest>(dto);
        withdrawalRequest.CreatorId = 2;//todo: poner id de usuario por token
        withdrawalRequest.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(withdrawalRequest);
        // TODO: agregar lógica para validar el monto permitido por la tienda
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> ApproveAsync(int WithdrawalRequestCreateId, int userId, WithdrawalRequestApproveDto dto)
    {
        WithdrawalRequest? withdrawalRequest = await _repository.GetByIdWithDetailsAsync(WithdrawalRequestCreateId);
        if (withdrawalRequest == null)
            return ApiResponse<bool>.Fail("Not found");

        await _walletTransactionService.RegisterSuccessfulWithdrawalRequestAcceptedAsync(withdrawalRequest);

        withdrawalRequest.Status = WithdrawalRequestStatus.Accepted;
        withdrawalRequest.ProcessedAt = DateTime.UtcNow;
        withdrawalRequest.UpdatedAt = DateTime.UtcNow;
        withdrawalRequest.ReviewedByUserId = userId; //todo: poner id de usuario por token
        withdrawalRequest.Reason = dto.Reason;
        await _repository.UpdateAsync(withdrawalRequest);
        // // todo: agregar lógica para validar el monto permitido por la tienda
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> RejectAsync(int withdrawalRequestCreateId, int userId, WithdrawalRequestRejectDto dto)
    {
        WithdrawalRequest? withdrawalRequest = await _repository.GetByIdWithDetailsAsync(withdrawalRequestCreateId);
        if (withdrawalRequest == null)
            return ApiResponse<bool>.Fail("Not found");

        withdrawalRequest.ProcessedAt = DateTime.UtcNow;
        withdrawalRequest.Status = WithdrawalRequestStatus.Rejected;
        withdrawalRequest.UpdatedAt = DateTime.UtcNow;
        withdrawalRequest.ReviewedByUserId = userId; //todo: poner id de usuario por token
        withdrawalRequest.Reason = dto.Reason;
        await _repository.UpdateAsync(withdrawalRequest);
        return ApiResponse<bool>.Success(true);
    }
}
