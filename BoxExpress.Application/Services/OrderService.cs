using System.Net.WebSockets;
using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Constants;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Utilities;

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
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
    private readonly IWalletTransactionService _walletTransactionService;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IInventoryMovementService _inventoryMovementService;
    private readonly IWarehouseInventoryTransferService _warehouseInventoryTransferService;
    private readonly IInventoryHoldService _inventoryHoldService;
    private readonly IUnitOfWork _unitOfWork;


    public OrderService(
        IOrderRepository repository,
        IMapper mapper,
        IOrderCategoryRepository orderCategoryRepository,
        IOrderStatusRepository orderStatusRepository,
        IWalletTransactionRepository walletTransactionRepository,
        ITransactionTypeRepository transactionTypeRepository,
        IOrderStatusHistoryRepository orderStatusHistoryRepository,
        IOrderCategoryHistoryRepository orderCategoryHistoryRepository,
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IWalletTransactionService walletTransactionService,
        IInventoryMovementService inventoryMovementService,
        IOrderItemRepository orderItemRepository,
        IWarehouseInventoryTransferService warehouseInventoryTransferService,
        IInventoryHoldService inventoryHoldService,
        IUnitOfWork unitOfWork
        )
    {
        _warehouseInventoryTransferService = warehouseInventoryTransferService;
        _orderCategoryHistoryRepository = orderCategoryHistoryRepository;
        _walletTransactionRepository = walletTransactionRepository;
        _orderStatusHistoryRepository = orderStatusHistoryRepository;
        _transactionTypeRepository = transactionTypeRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _inventoryHoldService = inventoryHoldService;
        _inventoryMovementService = inventoryMovementService;
        _walletTransactionService = walletTransactionService;
        _orderCategoryRepository = orderCategoryRepository;
        _orderStatusRepository = orderStatusRepository;
        _orderItemRepository = orderItemRepository;
        _repository = repository;
        _mapper = mapper;
        _unitOfWork = unitOfWork;
    }

    public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto filter)
    {
        var (orders, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<OrderFilter>(filter));
        return ApiResponse<IEnumerable<OrderDto>>.Success(_mapper.Map<List<OrderDto>>(orders), new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<IEnumerable<OrderSummaryDto>>> GetSummaryAsync(OrderFilterDto filter)
    {
        var summary = await _repository.GetSummaryAsync(_mapper.Map<OrderFilter>(filter));
        return ApiResponse<IEnumerable<OrderSummaryDto>>.Success(_mapper.Map<List<OrderSummaryDto>>(summary));
    }

    public async Task<ApiResponse<OrderDto?>> GetByIdAsync(int id) =>
        ApiResponse<OrderDto?>.Success(_mapper.Map<OrderDto>(await _repository.GetByIdWithDetailsAsync(id)));

    public async Task<ApiResponse<OrderDto>> UpdateWarehouseAsync(int orderId, int warehouseId)
    {
        Order? order = await _repository.GetByIdWithDetailsAsync(orderId);
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
            CreatedAt = DateTime.UtcNow,
            OrderId = order.Id,
            OldCategoryId = order.OrderCategoryId,
            NewCategoryId = (int)newCategoryId,
            CreatorId = 2, //todo tomar del token
        });

        order.OrderCategoryId = (int)newCategoryId;
        order.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<OrderDto>> UpdateStatusAsync(int orderId, int statusId)
    {
        #region validations 
        Order? order = await _repository.GetByIdWithDetailsAsync(orderId);
        if (order == null)
            return ApiResponse<OrderDto>.Fail("Order not found");

        if (order.OrderStatusId.Equals(statusId))
            return ApiResponse<OrderDto>.Fail("Same status");

        OrderStatus? orderStatus = await _orderStatusRepository.GetByIdAsync(statusId);
        if (orderStatus == null)
            return ApiResponse<OrderDto>.Fail("Status not found");

        var pendingHolds = await _inventoryHoldService.GetAllAsync(new InventoryHoldFilterDto
        {
            OrderId = orderId,
            Status = Domain.Enums.InventoryHoldStatus.PendingReturn
        });

        if (pendingHolds.Data != null && pendingHolds.Data.Any())
        {
            return ApiResponse<OrderDto>.Fail("El pedido tiene retenciones de inventario pendientes, resuélvalas antes de cambiar el estado.");
        }

        #endregion

        switch (orderStatus.Name)
        {
            case OrderStatusConstants.Delivered:
                await _inventoryMovementService.ProcessDeliveryAsync(order);
                await _walletTransactionService.RegisterSuccessfulDeliveryAsync(order, statusId);
                break;
            default:
                if (order.Status.Name.Equals(OrderStatusConstants.Delivered))
                {
                    await _inventoryMovementService.RevertDeliveryAsync(order);
                    await _walletTransactionService.RegisterStatusCorrectionAsync(order, statusId);
                }

                if (orderStatus.Name.Equals(OrderStatusConstants.Scheduled))
                {
                    var ReserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId!.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.Active);
                    if (!ReserveInventory.IsSuccess)
                        return ApiResponse<OrderDto>.Fail(ReserveInventory.Message ?? "Inventory not available");
                }

                //si la orden es cancelada y tiene bodega asignada, se reserva el hold en PendingReturn el inventario
                if (orderStatus.Name.Equals(OrderStatusConstants.Cancelled) || orderStatus.Name.Equals(OrderStatusConstants.CancelledAlt) && order.WarehouseId.HasValue)
                {
                    var ReserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.PendingReturn);
                    if (!ReserveInventory.IsSuccess)
                        return ApiResponse<OrderDto>.Fail(ReserveInventory.Message ?? "Inventory not available");
                }

                break;
        }

        await _orderStatusHistoryRepository.AddAsync(new()
        {
            OrderId = order.Id,
            OldStatusId = order.OrderStatusId,
            NewStatusId = statusId,
            CreatedAt = DateTime.UtcNow,
            CreatorId = 2 //todo: tomar del token
        });

        order.OrderStatusId = statusId;
        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<OrderDto>> UpdateScheduleAsync(int orderId, OrderScheduleUpdateDto orderScheduleUpdateDto)
    {
        if (orderScheduleUpdateDto.StatusId.HasValue)
        {
            await UpdateStatusAsync(orderId, orderScheduleUpdateDto.StatusId.Value);
        }

        Order? order = await _repository.GetByIdWithDetailsAsync(orderId);
        if (order == null)
        {
            return ApiResponse<OrderDto>.Fail("Order not found");
        }

        order.ScheduledDate = orderScheduleUpdateDto.ScheduledDate ?? order.ScheduledDate;
        order.TimeSlotId = orderScheduleUpdateDto.TimeSlotId ?? order.TimeSlotId;
        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<List<OrderStatusHistoryDto>>> GetStatusHistoryAsync(int orderId)
    {
        var listHistory = _mapper.Map<List<OrderStatusHistoryDto>>(await _orderStatusHistoryRepository.GetByOrderIdAsync(orderId));
        return ApiResponse<List<OrderStatusHistoryDto>>.Success(listHistory);
    }

    public async Task<ApiResponse<List<OrderCategoryHistoryDto>>> GetCategoryHistoryAsync(int orderId)
    {
        var listHistory = _mapper.Map<List<OrderCategoryHistoryDto>>(await _orderCategoryHistoryRepository.GetByOrderIdAsync(orderId));
        return ApiResponse<List<OrderCategoryHistoryDto>>.Success(listHistory);
    }

    public async Task<ApiResponse<List<OrderItemDto>>> GetProductsAsync(int orderId)
    {
        var listOrederItems = _mapper.Map<List<OrderItemDto>>(await _orderItemRepository.GetByOrderIdAsync(orderId));
        return ApiResponse<List<OrderItemDto>>.Success(listOrederItems);
    }

    public async Task<ApiResponse<bool>> AddOrderAsync(CreateOrderDto createOrderDto)
    {
        try
        {
            var createdAt = DateTime.UtcNow;
            var order = _mapper.Map<Order>(createOrderDto);
            order.CreatedAt = createdAt;
            await _unitOfWork.Orders.AddAsync(order);

            var inventories = await _warehouseInventoryRepository.GetByWarehouseAndProductVariants(createOrderDto.WarehouseId, createOrderDto.OrderItems.Select(p => p.ProductVariantId).ToList());


            //crear items de la orden
            foreach (var orderItemDto in createOrderDto.OrderItems)
            {
                var inventory = inventories.Where(i=> i.Id.Equals(orderItemDto.ProductVariantId)).FirstOrDefault();
                if (inventory == null || inventory.Quantity< orderItemDto.Quantity) 
                {
                    throw new Exception($"El item: {orderItemDto.ProductVariantName} no tiene suficiente inventario");

                }

                var oderItem = _mapper.Map<OrderItem>(orderItemDto);
                oderItem.OrderId = order.Id;
                oderItem.CreatedAt = createdAt;
                await _unitOfWork.OrderItems.AddAsync(oderItem);
            }

            //crear status history order
            var OrderStatusHistory = new OrderStatusHistory
            {
                CreatedAt = createdAt,
                OrderId = order.Id,
                CreatorId = createOrderDto.CreatorId,
                NewStatusId = createOrderDto.OrderStatusId,
            };

            await _unitOfWork.OrderStatusHistories.AddAsync(OrderStatusHistory);

            //crear category history order
            var categoryOrderHistory = new OrderCategoryHistory
            {
                CreatedAt = createdAt,
                OrderId= order.Id,
                CreatorId= createOrderDto.CreatorId,
                NewCategoryId = createOrderDto.CategoryId,
            };

            await _unitOfWork.OrderCategoryHistories.AddAsync(categoryOrderHistory);

            // crear walletTransaction pendiente definir 
            var wallet = new WalletTransaction
            {
                CreatedAt = createdAt,
                Amount = order.TotalAmount,
                TransactionTypeId = 2, //Todo llegara por el request o ser siempre 2  
                WalletId = createOrderDto.WalletId,
                CreatorId = createOrderDto.CreatorId,
                OrderStatusId = order.OrderStatusId,
            };

            await _unitOfWork.WalletTransactions.AddAsync(wallet);

            return ApiResponse<bool>.Success(true, null, "Orden creada exitosamente");

        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<bool>.Fail("Error al crear Orden: " + ex.Message);
        }
    }
}
