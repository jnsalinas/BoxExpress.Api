using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Constants;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services;

public class OrderService : IOrderService
{
    private readonly IMapper _mapper;

    private readonly IOrderRepository _repository;
    private readonly ITransactionTypeRepository _transactionTypeRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IOrderCategoryRepository _orderCategoryRepository;
    private readonly IWalletTransactionRepository _walletTransactionRepository;
    private readonly IOrderStatusHistoryRepository _orderStatusHistoryRepository;
    private readonly IOrderCategoryHistoryRepository _orderCategoryHistoryRepository;

    public OrderService(
        IOrderRepository repository,
        IMapper mapper,
        IOrderCategoryRepository orderCategoryRepository,
        IOrderStatusRepository orderStatusRepository,
        IWalletTransactionRepository walletTransactionRepository,
        ITransactionTypeRepository transactionTypeRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository,
        IOrderCategoryHistoryRepository orderCategoryHistoryRepository
        )
    {
        _orderCategoryHistoryRepository = orderCategoryHistoryRepository;
        _walletTransactionRepository = walletTransactionRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _orderCategoryRepository = orderCategoryRepository;
        _transactionTypeRepository = transactionTypeRepository;
        _orderStatusRepository = orderStatusRepository;
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto filter) =>
             ApiResponse<IEnumerable<OrderDto>>.Success(_mapper.Map<List<OrderDto>>(await _repository.GetFilteredAsync(_mapper.Map<OrderFilter>(filter))));

    public async Task<ApiResponse<OrderDetailDto?>> GetByIdAsync(int id) =>
        ApiResponse<OrderDetailDto?>.Success(_mapper.Map<OrderDetailDto>(await _repository.GetByIdWithDetailsAsync(id)));

    public async Task<ApiResponse<OrderDto>> UpdateWarehouseAsync(int orderId, int warehouseId)
    {
        Order? order = await _repository.GetByIdAsync(orderId);
        if (order == null)
            return ApiResponse<OrderDto>.Fail("Order not found");

        int? newCategoryId;
        if (warehouseId.Equals(0))
        {
            newCategoryId = (await _orderCategoryRepository.GetByNameAsync(OrderCategoryConstants.Traditional))?.Id;
        }
        else
        {
            newCategoryId = (await _orderCategoryRepository.GetByNameAsync(OrderCategoryConstants.Express))?.Id;
            order.WarehouseId = warehouseId;
        }

        if (!newCategoryId.HasValue)
            return ApiResponse<OrderDto>.Fail("Order category not found");

        //log
        await _orderCategoryHistoryRepository.AddAsync(new()
        {
            CreatedAt = DateTime.Now,
            OrderId = order.Id,
            OldCategoryId = order.OrderCategoryId,
            NewCategoryId = (int)newCategoryId,
            CreatorId = 2, //todo tomar del token
        });

        order.OrderCategoryId = (int)newCategoryId;
        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<OrderDto>> UpdateStatusAsync(int orderId, int statusId)
    {
        #region validations 
        Order? order = await _repository.GetByIdWithDetailsAsync(orderId);
        if (order == null)
            return ApiResponse<OrderDto>.Fail("Order not found");

        OrderStatus? orderStatus = await _orderStatusRepository.GetByIdAsync(statusId);
        if (orderStatus == null)
            return ApiResponse<OrderDto>.Fail("Status not found");
        #endregion

        //Todo aca van solo los que mueven la wallet
        TransactionType? transactionType = null;
        switch (orderStatus.Name)
        {
            case OrderStatusConstants.Delivered:
                transactionType = await _transactionTypeRepository.GetByNameAsync(TransactionTypeConstants.Inbound);
                //hacer operacion con la wallet el balance
                break;
            case OrderStatusConstants.Cancelled:
                //todo si previamente estaba entregada debe mover wallet 
                transactionType = await _transactionTypeRepository.GetByNameAsync(TransactionTypeConstants.Outbound);
                //hacer operacion con la wallet el balance
                break;
        }

        if (transactionType != null)
        {
            //todo comenazar a validar logica de estados, si era antes entregado el cancelado debe mover la wallet
            //todo validar que mas logica tiene el wallettransaction y si es necesario crear el wallettransactionservice para que maneje toda la logica
            await _walletTransactionRepository.AddAsync(new()
            {
                CreatedAt = DateTime.Now,
                WalletId = order.Store.WalletId,
                TransactionTypeId = transactionType.Id, //todo en el codigo de smartmonkey hay entrada y salida
                Amount = order.TotalAmount,
                Description = WalletDescriptionConstants.SuccessfulDeliveryPrefix + orderId,
                RelatedOrderId = order.Id,
                CreatorId = 2, //todo tomar del token
            });
        }

        //todo guardar log de cambio de estado
        await _orderStatusHistoryRepository.AddAsync(new()
        {
            OrderId = order.Id,
            OldStatusId = order.OrderStatusId,
            NewStatusId = statusId,
            CreatedAt = DateTime.Now,
            CreatorId = 2 //todo tomar del token
        });

        order.OrderStatusId = statusId;
        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }


}
