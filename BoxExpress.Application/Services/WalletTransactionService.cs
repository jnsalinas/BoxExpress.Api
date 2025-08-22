using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Constants;
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Services;

public class WalletTransactionService : IWalletTransactionService
{
    private readonly IWalletTransactionRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITransactionTypeRepository _transactionTypeRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IInventoryMovementService _inventoryMovementService;
    private readonly IUserContext _userContext;

    public WalletTransactionService(
        IWalletTransactionRepository repository,
        ITransactionTypeRepository transactionTypeRepository,
        IInventoryMovementService inventoryMovementService,
        IWalletRepository walletRepository,
        IMapper mapper,
        IUserContext userContext)
    {
        _transactionTypeRepository = transactionTypeRepository;
        _inventoryMovementService = inventoryMovementService;
        _walletRepository = walletRepository;
        _repository = repository;
        _mapper = mapper;
        _userContext = userContext;
    }

    public async Task<ApiResponse<IEnumerable<WalletTransactionDto>>> GetAllAsync(WalletTransactionFilterDto filter)
    {
        var (transactions, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<WalletTransactionFilter>(filter));
        return ApiResponse<IEnumerable<WalletTransactionDto>>.Success(_mapper.Map<List<WalletTransactionDto>>(transactions), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task RegisterSuccessfulDeliveryAsync(Order order, int orderStatusId)
    {
        //todo: organizar codigo, evitar repeticion
        var types = await _transactionTypeRepository.GetAllAsync();

        var inbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Inbound);
        var outbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Outbound);

        if (inbound == null || outbound == null)
        {
            throw new InvalidOperationException("Transaction types not found");
        }

        //move balance 
        //todo: mirar si se pasa a wallet service 
        Wallet? wallet = await _walletRepository.GetByStoreIdAsync(order.StoreId) ?? throw new InvalidOperationException("Wallet not found");
        wallet.Balance += order.TotalAmount - order.DeliveryFee;
        wallet.UpdatedAt = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);

        // Register the successful delivery transaction
        await _repository.AddAsync(new WalletTransaction
        {
            WalletId = order.Store.WalletId,
            TransactionTypeId = inbound.Id,
            Amount = order.TotalAmount, 
            Description = string.Format(WalletDescriptionConstants.SuccessfulDeliveryPrefix, order.Id),
            RelatedOrderId = order.Id,
            OrderStatusId = orderStatusId,
            CreatorId = _userContext.UserId.Value,
        });

        // Register the delivery fee transaction
        await _repository.AddAsync(new WalletTransaction
        {
            WalletId = order.Store.WalletId,
            TransactionTypeId = outbound.Id,
            Amount = order.DeliveryFee,
            Description = string.Format(WalletDescriptionConstants.DeliveredOrderDiscountPrefix, order.Id),
            RelatedOrderId = order.Id,
            OrderStatusId = orderStatusId,
            CreatorId = _userContext.UserId.Value,
        });
    }

    public async Task RegisterStatusCorrectionAsync(Order order, int orderStatusId)
    {
        //todo: organizar codigo, evitar repeticion
        var types = await _transactionTypeRepository.GetAllAsync();

        var inbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Inbound);
        var outbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Outbound);

        if (inbound == null || outbound == null)
        {
            throw new InvalidOperationException("Transaction types not found");
        }

        //todo: mirar si se pasa a wallet service 
        Wallet? wallet = await _walletRepository.GetByStoreIdAsync(order.StoreId) ?? throw new InvalidOperationException("Wallet not found");
        wallet.Balance -= order.TotalAmount - order.DeliveryFee;
        wallet.UpdatedAt = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);

        // Register the status correction transaction
        await _repository.AddAsync(new WalletTransaction
        {
            WalletId = order.Store.WalletId,
            TransactionTypeId = outbound.Id,
            Amount = order.TotalAmount,
            Description = string.Format(WalletDescriptionConstants.DiscountForOrderStatusCorrection, order.Id),
            RelatedOrderId = order.Id,
            OrderStatusId = orderStatusId,
            CreatorId = _userContext.UserId.Value,
        });

        // Register the refund transaction
        await _repository.AddAsync(new WalletTransaction
        {
            WalletId = order.Store.WalletId,
            TransactionTypeId = inbound.Id,
            Amount = order.DeliveryFee,
            Description = string.Format(WalletDescriptionConstants.RefundForOrderStatusCorrection, order.Id),
            RelatedOrderId = order.Id,
            OrderStatusId = orderStatusId,
            CreatorId = _userContext.UserId.Value,
        });
    }

    public async Task RegisterSuccessfulWithdrawalRequestAcceptedAsync(WithdrawalRequest withdrawalRequest)
    {
        //todo organizar codigo, evitar repeticion
        var types = await _transactionTypeRepository.GetAllAsync();

        var outbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Outbound);

        if (outbound == null)
        {
            throw new InvalidOperationException("Transaction types not found");
        }

        //todo: mirar si se pasa a wallet service 
        Wallet? wallet = await _walletRepository.GetByStoreIdAsync(withdrawalRequest.StoreId) ?? throw new InvalidOperationException("Wallet not found");
        wallet.Balance -= withdrawalRequest.Amount;
        wallet.PendingWithdrawals -= withdrawalRequest.Amount;
        wallet.UpdatedAt = DateTime.UtcNow;
        await _walletRepository.UpdateAsync(wallet);

        // Register the successful delivery transaction
        await _repository.AddAsync(new WalletTransaction
        {
            WalletId = withdrawalRequest.Store.WalletId,
            TransactionTypeId = outbound.Id,
            Amount = withdrawalRequest.Amount,
            Description = string.Format(WalletDescriptionConstants.WithdrawalRequestAccepted, withdrawalRequest.Id),
            RelatedWithdrawalRequestId = withdrawalRequest.Id,
            CreatorId = _userContext.UserId.Value,
        });
    }
}
