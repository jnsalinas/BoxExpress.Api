using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Services;

public class WithdrawalRequestService : IWithdrawalRequestService
{
    private readonly IWithdrawalRequestRepository _repository;
    private readonly IMapper _mapper;
    private readonly IWalletTransactionService _walletTransactionService;
    private readonly IWalletRepository _walletRepository;

    public WithdrawalRequestService(
        IWithdrawalRequestRepository repository,
        IMapper mapper,
        IWalletTransactionService walletTransactionService,
        IWalletRepository walletRepository)
    {
        _walletTransactionService = walletTransactionService;
        _repository = repository;
        _mapper = mapper;
        _walletRepository = walletRepository;
    }

    public async Task<ApiResponse<IEnumerable<WithdrawalRequestDto>>> GetAllAsync(WithdrawalRequestFilterDto filter)
    {
        var (withdrawalRequest, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<WithdrawalRequestFilter>(filter));
        return ApiResponse<IEnumerable<WithdrawalRequestDto>>.Success(_mapper.Map<List<WithdrawalRequestDto>>(withdrawalRequest), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<bool>> AddAsync(WithdrawalRequestCreateDto dto)
    {
        if (!dto.StoreId.HasValue)
        {
            return ApiResponse<bool>.Fail("Tienda es requerida");
        }
        if (dto.Amount <= 0)
        {
            return ApiResponse<bool>.Fail("Monto debe ser mayor a 0");
        }

        //add retiros pendientes
        Wallet wallet = await _walletRepository.GetByStoreIdAsync(dto.StoreId.Value) ?? throw new InvalidOperationException("Wallet not found");
        if (wallet.Balance < dto.Amount)
            return ApiResponse<bool>.Fail("Balance insuficiente para realizar el retiro");

        wallet.PendingWithdrawals += dto.Amount;
        await _walletRepository.UpdateAsync(wallet);

        WithdrawalRequest withdrawalRequest = _mapper.Map<WithdrawalRequest>(dto);
        withdrawalRequest.CreatorId = 2;//todo: poner id de usuario por token
        withdrawalRequest.CreatedAt = DateTime.UtcNow;
        await _repository.AddAsync(withdrawalRequest);
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> ApproveAsync(int WithdrawalRequestCreateId, int userId, WithdrawalRequestApproveDto dto)
    {
        WithdrawalRequest? withdrawalRequest = await _repository.GetByIdWithDetailsAsync(WithdrawalRequestCreateId);
        if (withdrawalRequest == null)
            return ApiResponse<bool>.Fail("Not found");

        await _walletTransactionService.RegisterSuccessfulWithdrawalRequestAcceptedAsync(withdrawalRequest);
        // // todo: agregar lógica para validar el monto permitido por la tienda

        withdrawalRequest.Status = WithdrawalRequestStatus.Accepted;
        withdrawalRequest.ProcessedAt = DateTime.UtcNow;
        withdrawalRequest.UpdatedAt = DateTime.UtcNow;
        withdrawalRequest.ReviewedByUserId = userId; //todo: poner id de usuario por token
        withdrawalRequest.Reason = dto.Reason;
        await _repository.UpdateAsync(withdrawalRequest);
        return ApiResponse<bool>.Success(true);
    }

    public async Task<ApiResponse<bool>> RejectAsync(int withdrawalRequestCreateId, int userId, WithdrawalRequestRejectDto dto)
    {
        WithdrawalRequest? withdrawalRequest = await _repository.GetByIdWithDetailsAsync(withdrawalRequestCreateId);
        if (withdrawalRequest == null)
            return ApiResponse<bool>.Fail("Not found");

        //todo: mirar si se pasa a wallet service 
        Wallet? wallet = await _walletRepository.GetByStoreIdAsync(withdrawalRequest.StoreId) ?? throw new InvalidOperationException("Wallet not found");
        wallet.PendingWithdrawals -= withdrawalRequest.Amount;
        wallet.UpdatedAt = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);

        withdrawalRequest.ProcessedAt = DateTime.UtcNow;
        withdrawalRequest.Status = WithdrawalRequestStatus.Rejected;
        withdrawalRequest.UpdatedAt = DateTime.UtcNow;
        withdrawalRequest.ReviewedByUserId = userId; //todo: poner id de usuario por token
        withdrawalRequest.Reason = dto.Reason;
        await _repository.UpdateAsync(withdrawalRequest);
        return ApiResponse<bool>.Success(true);
    }
}
