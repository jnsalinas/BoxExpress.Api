using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Constants;

namespace BoxExpress.Application.Services;

public class WalletTransactionService : IWalletTransactionService
{
    private readonly IWalletTransactionRepository _repository;
    private readonly IMapper _mapper;
    private readonly ITransactionTypeRepository _transactionTypeRepository;

    public WalletTransactionService(IWalletTransactionRepository repository, ITransactionTypeRepository transactionTypeRepository, IMapper mapper)
    {
        _transactionTypeRepository = transactionTypeRepository;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<WalletTransactionDto>>> GetAllAsync(WalletTransactionFilterDto filter)
    {
        var (transactions, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<WalletTransactionFilter>(filter));
        return ApiResponse<IEnumerable<WalletTransactionDto>>.Success(_mapper.Map<List<WalletTransactionDto>>(transactions), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task RegisterSuccessfulDeliveryAsync(Order order, int orderStatusId)
    {
        //todo organizar codigo, evitar repeticion
        var types = await _transactionTypeRepository.GetAllAsync();

        var inbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Inbound);
        var outbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Outbound);

        if (inbound == null || outbound == null)
        {
            throw new InvalidOperationException("Transaction types not found");
        }
        

        // Register the successful delivery transaction
        await _repository.AddAsync(new WalletTransaction
        {
            WalletId = order.Store.WalletId,
            TransactionTypeId = inbound.Id,
            Amount = order.TotalAmount,
            Description = string.Format(WalletDescriptionConstants.SuccessfulDeliveryPrefix, order.Id),
            RelatedOrderId = order.Id,
            OrderStatusId = orderStatusId,
            CreatorId = 2,
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
            CreatorId = 2,
        });
    }

    public async Task RegisterStatusCorrectionAsync(Order order, int orderStatusId)
    {
        //todo organizar codigo, evitar repeticion
        var types = await _transactionTypeRepository.GetAllAsync();

        var inbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Inbound);
        var outbound = types.FirstOrDefault(x => x.Name == TransactionTypeConstants.Outbound);

        if (inbound == null || outbound == null)
        {
            throw new InvalidOperationException("Transaction types not found");
        }

        // Register the status correction transaction
        await _repository.AddAsync(new WalletTransaction
        {
            WalletId = order.Store.WalletId,
            TransactionTypeId = outbound.Id,
            Amount = order.TotalAmount,
            Description = string.Format(WalletDescriptionConstants.DiscountForOrderStatusCorrection, order.Id),
            RelatedOrderId = order.Id,
            OrderStatusId = orderStatusId,
            CreatorId = 2,
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
            CreatorId = 2,
        });
    }
}
