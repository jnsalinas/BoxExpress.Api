using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IWalletTransactionService
{
    Task<ApiResponse<IEnumerable<WalletTransactionDto>>> GetAllAsync(WalletTransactionFilterDto filter);
    Task RegisterSuccessfulDeliveryAsync(Order order, int orderStatusId);
    Task RegisterStatusCorrectionAsync(Order order, int orderStatusId);
    Task RegisterSuccessfulWithdrawalRequestAcceptedAsync(WithdrawalRequest withdrawalRequest);
}
