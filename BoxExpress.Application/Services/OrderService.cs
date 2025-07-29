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
using Microsoft.Extensions.Configuration;
using ClosedXML.Excel;

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
    private readonly IClientAddressRepository _clientAddressRepository;
    private readonly IClientRepository _clientRepository;
    private readonly IDocumentTypeRepository _documentTypeRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IProductVariantRepository _productVariantRepository;
    private readonly IConfiguration _configuration;
    private readonly IUserContext _userContext;
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
        IClientAddressRepository clientAddressRepository,
        IClientRepository clientRepository,
        IDocumentTypeRepository documentTypeRepository,
        IUnitOfWork unitOfWork,
        IProductVariantRepository productVariantRepository,
        IConfiguration configuration,
        IUserContext userContext
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
        _productVariantRepository = productVariantRepository;
        _orderItemRepository = orderItemRepository;
        _repository = repository;
        _clientAddressRepository = clientAddressRepository;
        _clientRepository = clientRepository;
        _documentTypeRepository = documentTypeRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
        _mapper = mapper;
        _userContext = userContext;
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
            CreatorId = _userContext.UserId, //todo tomar del token
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

        //ahora puede tener varios en hold ya que la orden puede volver a ser programada
        // if (pendingHolds.Data != null && pendingHolds.Data.Any())
        // {
        //     return ApiResponse<OrderDto>.Fail("El pedido tiene retenciones de inventario pendientes, resuélvalas antes de cambiar el estado.");
        // }

        #endregion

        switch (orderStatus.Name)
        {
            case OrderStatusConstants.Delivered:
                //todo: validar si la orden estaba devuelta y tiene items por devolucion que tome el ultimo y lo revierta supongo
                await _inventoryMovementService.ProcessDeliveryAsync(order);
                await _walletTransactionService.RegisterSuccessfulDeliveryAsync(order, statusId);
                break;
            default:
                bool isCanceled = orderStatus.Name.Equals(OrderStatusConstants.Cancelled) || orderStatus.Name.Equals(OrderStatusConstants.CancelledAlt) && order.WarehouseId.HasValue;
                if (order.Status.Name.Equals(OrderStatusConstants.Delivered))
                {
                    await _inventoryMovementService.RevertDeliveryAsync(order);
                    await _walletTransactionService.RegisterStatusCorrectionAsync(order, statusId);

                    //vuelve a reservar del inventario para luego validar si es cancelado y reusar lo mismo cuando previamente no tiene el estado de entregado
                    if (isCanceled)
                    {
                        var ReserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId!.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.Active);
                        if (!ReserveInventory.IsSuccess)
                            return ApiResponse<OrderDto>.Fail(ReserveInventory.Message ?? "Inventory not available");
                    }
                }

                //si la orden es progrmaada debe pasar al modulo de gestion para que la puedan poner en ruta 
                if (orderStatus.Name.Equals(OrderStatusConstants.Scheduled))
                {

                }

                if (orderStatus.Name.Equals(OrderStatusConstants.OnTheWay))
                {
                    var ReserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId!.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.Active);
                    if (!ReserveInventory.IsSuccess)
                        return ApiResponse<OrderDto>.Fail(ReserveInventory.Message ?? "Inventory not available");
                }

                //si la orden es cancelada y tiene bodega asignada, se reserva el hold en PendingReturn el inventario
                if (isCanceled)
                {
                    var reserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.PendingReturn);
                    if (!reserveInventory.IsSuccess)
                        return ApiResponse<OrderDto>.Fail(reserveInventory.Message ?? "Inventory not available");
                }

                break;
        }

        await _orderStatusHistoryRepository.AddAsync(new()
        {
            OrderId = order.Id,
            OldStatusId = order.OrderStatusId,
            NewStatusId = statusId,
            CreatedAt = DateTime.UtcNow,
            CreatorId = _userContext.UserId
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

    public async Task<ApiResponse<OrderDto>> AddOrderAsync(CreateOrderDto createOrderDto)
    {
        try
        {
            //todo quitar
            var createdAt = DateTime.UtcNow;

            // Create new client
            var client = new Client
            {
                FirstName = createOrderDto.ClientFirstName,
                LastName = createOrderDto.ClientLastName,
                // Document = createOrderDto.ClientDocument,
                Email = createOrderDto.ClientEmail,
                // ExternalId = createOrderDto.ClientExternalId,
                CreatedAt = createdAt,
                Phone = createOrderDto.ClientPhone ?? string.Empty,
                // DocumentTypeId = createOrderDto.ClientDocumentTypeId,
            };
            await _unitOfWork.Clients.AddAsync(client);

            // Create client address
            var clientAddress = new ClientAddress
            {
                ClientId = client.Id,
                Address = createOrderDto.ClientAddress,
                Complement = createOrderDto.ClientAddressComplement,
                Address2 = createOrderDto.ClientAddress2,
                CityId = createOrderDto.CityId,
                Latitude = !string.IsNullOrEmpty(createOrderDto.Latitude) ? decimal.Parse(createOrderDto.Latitude) : null,
                Longitude = !string.IsNullOrEmpty(createOrderDto.Longitude) ? decimal.Parse(createOrderDto.Longitude) : null,
                IsDefault = true,
                CreatedAt = createdAt,
                PostalCode = createOrderDto.PostalCode,
            };
            await _unitOfWork.ClientAddresses.AddAsync(clientAddress);

            // Get default order status and category
            var defaultOrderStatus = await _orderStatusRepository.GetByNameAsync(OrderStatusConstants.Unscheduled);
            //var defaultOrderCategory = await _orderCategoryRepository.GetByNameAsync(OrderCategoryConstants.Traditional);

            if (defaultOrderStatus == null)
            {
                return ApiResponse<OrderDto>.Fail("Estado de orden por defecto no encontrado");
            }

            if (!string.IsNullOrEmpty(createOrderDto.Code))
            {
                var orderExist = await _repository.GetByCodeAsync(createOrderDto.Code, createOrderDto.StoreId);
                if (orderExist != null)
                {
                    return ApiResponse<OrderDto>.Fail($"Orden {orderExist.Code} ya existe", _mapper.Map<OrderDto>(orderExist));
                }
            }

            var order = new Order
            {
                StoreId = createOrderDto.StoreId,
                OrderStatusId = defaultOrderStatus.Id,
                DeliveryFee = createOrderDto.DeliveryFee ?? 0,
                CurrencyId = createOrderDto.CurrencyId,
                ClientId = client.Id,
                ClientAddressId = clientAddress.Id,
                CityId = createOrderDto.CityId,
                Code = createOrderDto.Code,
                Contains = createOrderDto.Contains,
                TotalAmount = createOrderDto.TotalAmount,
                Notes = createOrderDto.Notes,
                CreatedAt = createdAt,
                CreatorId = _userContext.UserId,
                IsEnabled = true,
            };

            await _unitOfWork.Orders.AddAsync(order);

            // Validate and create order items
            foreach (var orderItemDto in createOrderDto.OrderItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.Id,
                    ProductVariantId = orderItemDto.ProductVariantId,
                    Quantity = orderItemDto.Quantity ?? 0,
                    CreatedAt = createdAt
                };
                await _unitOfWork.OrderItems.AddAsync(orderItem);
            }

            // Create order status history
            var orderStatusHistory = new OrderStatusHistory
            {
                CreatedAt = createdAt,
                OrderId = order.Id,
                CreatorId = createOrderDto.CreatorId ?? 0,
                NewStatusId = defaultOrderStatus.Id
            };
            await _unitOfWork.OrderStatusHistories.AddAsync(orderStatusHistory);
            await _unitOfWork.CommitAsync();

            return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order), null, "Orden creada exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<OrderDto>.Fail("Error al crear Orden: " + ex.Message);
        }
    }

    public async Task<ApiResponse<List<OrderExcelUploadResponseDto>>> AddOrdersFromExcelAsync(OrderExcelUploadDto dto)
    {
        using var stream = new MemoryStream(dto.Content);
        using var workbook = new XLWorkbook(stream);
        var ordersSheet = workbook.Worksheet(1);

        var orders = new List<CreateOrderDto>();
        var allSkus = new HashSet<string>();

        var lastRow = ordersSheet.LastRowUsed().RowNumber();

        for (int row = 2; row <= lastRow; row++)
        {
            var skuCell = ordersSheet.Cell(row, 15).GetString();
            var orderItemsSKUS = skuCell.Split(';');

            foreach (var sku in orderItemsSKUS)
            {
                var skuCode = sku.Split(':')[0].Trim();
                if (!string.IsNullOrEmpty(skuCode))
                    allSkus.Add(skuCode);
            }
        }

        var skuToVariant = await _warehouseInventoryRepository.GetBySkusAsync(allSkus, dto.StoreId);

        for (int row = 2; row <= lastRow; row++)
        {
            int col = 1;
            var order = new CreateOrderDto
            {
                CreatorId = dto.CreatorId,
                StoreId = dto.StoreId,
                Code = ordersSheet.Cell(row, col++).GetString(),
                ClientFirstName = ordersSheet.Cell(row, col++).GetString(),
                ClientLastName = ordersSheet.Cell(row, col++).GetString(),
                ClientDocumentTypeId = int.Parse(ordersSheet.Cell(row, col++).GetString()),
                ClientDocument = ordersSheet.Cell(row, col++).GetString(),
                ClientEmail = ordersSheet.Cell(row, col++).GetString(),
                ClientPhone = ordersSheet.Cell(row, col++).GetString(),
                ClientAddress = ordersSheet.Cell(row, col++).GetString(),
                ClientAddressComplement = ordersSheet.Cell(row, col++).GetString(),
                ClientAddress2 = ordersSheet.Cell(row, col++).GetString(),
                Latitude = ordersSheet.Cell(row, col++).GetString(),
                Longitude = ordersSheet.Cell(row, col++).GetString(),
                TotalAmount = decimal.Parse(ordersSheet.Cell(row, col++).GetString()),
                Notes = $"Carga masiva: {ordersSheet.Cell(row, col++).GetString()}",
                OrderItems = new List<OrderItemDto>()
            };

            order.CityId = 1; //todo revisar como cargar esto
            order.CurrencyId = 1;

            var countryCode = "MX";
            var deliveryFeeSection = _configuration.GetSection($"{countryCode}:DeliveryFee");
            if (deliveryFeeSection.Exists() && decimal.TryParse(deliveryFeeSection.Value, out var fee))
            {
                order.DeliveryFee = fee;
            }
            else
            {
                order.DeliveryFee = 150;
            }

            var orderItemsSKUS = ordersSheet.Cell(row, col++).GetString().Split(';');
            foreach (var sku in orderItemsSKUS)
            {
                var parts = sku.Split(':');
                var skuCode = parts[0].Trim();
                var quantityText = parts.Length > 1 ? parts[1].Trim() : "0";

                var productVariant = skuToVariant
                    .FirstOrDefault(x => x.ProductVariant != null && x.ProductVariant.Sku != null && x.ProductVariant.Sku.Equals(skuCode, StringComparison.OrdinalIgnoreCase));
                if (productVariant != null)
                {
                    order.OrderItems.Add(new OrderItemDto
                    {
                        ProductVariantId = productVariant.Id,
                        Quantity = int.Parse(quantityText)
                    });
                }
            }

            orders.Add(order);
        }

        var result = new List<OrderExcelUploadResponseDto>();
        foreach (var order in orders)
        {
            var response = await AddOrderAsync(order);
            // var response = new ApiResponse<OrderDto>
            // {
            //     IsSuccess = true,
            //     Message = "Orden creada exitosamente",
            // };
            result.Add(new OrderExcelUploadResponseDto
            {
                Code = order.Code,
                Message = response.Message,
                IsLoaded = response.IsSuccess
            });
        }

        return ApiResponse<List<OrderExcelUploadResponseDto>>.Success(result, null, "Órdenes creadas exitosamente");
    }

}
