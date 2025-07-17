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
using OfficeOpenXml;

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
        IConfiguration configuration
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
        _mapper = mapper;
        _clientAddressRepository = clientAddressRepository;
        _clientRepository = clientRepository;
        _documentTypeRepository = documentTypeRepository;
        _unitOfWork = unitOfWork;
        _configuration = configuration;
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

            // Validate document type
            var documentType = await _documentTypeRepository.GetByIdAsync(createOrderDto.ClientDocumentTypeId);
            if (documentType == null)
            {
                return ApiResponse<bool>.Fail("Tipo de documento no encontrado");
            }

            // Find or create client
            var existingClient = await _clientRepository.GetByDocumentAsync(createOrderDto.ClientDocument);
            Client client;

            if (existingClient == null)
            {
                // Create new client
                client = new Client
                {
                    FirstName = createOrderDto.ClientFirstName,
                    LastName = createOrderDto.ClientLastName,
                    Document = createOrderDto.ClientDocument,
                    Email = createOrderDto.ClientEmail,
                    ExternalId = createOrderDto.ClientExternalId,
                    CreatedAt = createdAt,
                    Phone = createOrderDto.ClientPhone ?? string.Empty,
                    DocumentTypeId = createOrderDto.ClientDocumentTypeId,
                };
                await _unitOfWork.Clients.AddAsync(client);
            }
            else
            {
                // Update existing client information
                client = existingClient;
                client.FirstName = createOrderDto.ClientFirstName;
                client.LastName = createOrderDto.ClientLastName;
                client.Email = createOrderDto.ClientEmail;
                client.ExternalId = createOrderDto.ClientExternalId;
                client.UpdatedAt = createdAt;
                await _unitOfWork.Clients.UpdateAsync(client);
            }

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
                CreatedAt = createdAt
            };
            await _unitOfWork.ClientAddresses.AddAsync(clientAddress);

            // Get default order status and category
            var defaultOrderStatus = await _orderStatusRepository.GetByNameAsync(OrderStatusConstants.Unscheduled);
            //var defaultOrderCategory = await _orderCategoryRepository.GetByNameAsync(OrderCategoryConstants.Traditional);

            if (defaultOrderStatus == null)
            {
                return ApiResponse<bool>.Fail("Estado de orden por defecto no encontrado");
            }

            // if (defaultOrderCategory == null)
            // {
            //     return ApiResponse<bool>.Fail("Categoría de orden por defecto no encontrada");
            // }

            // Create order
            var order = new Order
            {
                StoreId = createOrderDto.StoreId,
                OrderStatusId = defaultOrderStatus.Id,
                // OrderCategoryId = defaultOrderCategory.Id,
                DeliveryFee = createOrderDto.DeliveryFee ?? 0,
                CurrencyId = createOrderDto.CurrencyId,
                ClientId = client.Id,
                ClientAddressId = clientAddress.Id,
                CityId = createOrderDto.CityId,
                Code = createOrderDto.Code ?? string.Empty,
                Contains = createOrderDto.Contains,
                TotalAmount = createOrderDto.TotalAmount,
                Notes = createOrderDto.Notes,
                ExternalId = createOrderDto.ExternalId,
                CreatedAt = createdAt,
                CreatorId = createOrderDto.CreatorId ?? 0,
                IsEnabled = true
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

            // Create order category history
            // var orderCategoryHistory = new OrderCategoryHistory
            // {
            //     CreatedAt = createdAt,
            //     OrderId = order.Id,
            //     CreatorId = createOrderDto.CreatorId ?? 0,
            //     NewCategoryId = defaultOrderCategory.Id
            // };
            // await _unitOfWork.OrderCategoryHistories.AddAsync(orderCategoryHistory);

            await _unitOfWork.CommitAsync();
            return ApiResponse<bool>.Success(true, null, "Orden creada exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<bool>.Fail("Error al crear Orden: " + ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> AddOrdersFromExcelAsync(Stream stream, int? storeId = null)
    {
        using var package = new ExcelPackage(stream);
        var ordersSheet = package.Workbook.Worksheets[0];

        var orders = new List<CreateOrderDto>();

        var allSkus = new HashSet<string>();
        for (int row = 2; row <= ordersSheet.Dimension.End.Row; row++)
        {
            var orderItemsSKUS = ordersSheet.Cells[row, 14].Text.Split(';');
            foreach (var sku in orderItemsSKUS)
            {
                var skuCode = sku.Split(':')[0].Trim();
                if (!string.IsNullOrEmpty(skuCode))
                    allSkus.Add(skuCode);
            }
        }

        var skuToVariant = await _productVariantRepository.GetBySkusAsync(allSkus);

        for (int row = 2; row <= ordersSheet.Dimension.End.Row; row++)
        {
            int col = 1;
            var order = new CreateOrderDto
            {
                Code = ordersSheet.Cells[row, col++].Text,
                ClientFirstName = ordersSheet.Cells[row, col++].Text,
                ClientLastName = ordersSheet.Cells[row, col++].Text,
                ClientDocumentTypeId = int.Parse(ordersSheet.Cells[row, col++].Text),
                ClientDocument = ordersSheet.Cells[row, col++].Text,
                ClientEmail = ordersSheet.Cells[row, col++].Text,
                ClientPhone = ordersSheet.Cells[row, col++].Text,
                ClientAddress = ordersSheet.Cells[row, col++].Text,
                ClientAddressComplement = ordersSheet.Cells[row, col++].Text,
                ClientAddress2 = ordersSheet.Cells[row, col++].Text,
                Latitude = ordersSheet.Cells[row, col++].Text,
                Longitude = ordersSheet.Cells[row, col++].Text,
                TotalAmount = decimal.Parse(ordersSheet.Cells[row, col++].Text),
                Notes = ordersSheet.Cells[row, col++].Text,
                OrderItems = new List<OrderItemDto>()
            };

            if (storeId != null)
            {
                order.StoreId = storeId.Value;
                col++;
            }
            else
            {
                order.StoreId = int.Parse(ordersSheet.Cells[row, col++].Text);
            }

            order.CityId = 1;
            order.CurrencyId = 1;
            // Obtener el valor de DeliveryFee desde la configuración por país (appsettings.json)
            var countryCode = "MX"; // TODO: obtener dinámicamente según la orden o el usuario
            var deliveryFeeSection = _configuration.GetSection($"{countryCode}:DeliveryFee");
            if (deliveryFeeSection.Exists() && decimal.TryParse(deliveryFeeSection.Value, out var fee))
            {
                order.DeliveryFee = fee;
            }
            else
            {
                order.DeliveryFee = 150;
            }

            // .
            //CurrencyId = int.Parse(ordersSheet.Cells[row, col++].Text),
            //CityId = 1, //int.Parse(ordersSheet.Cells[row, col++].Text),


            var orderItemsSKUS = ordersSheet.Cells[row, col++].Text.Split(';');
            foreach (var sku in orderItemsSKUS)
            {
                // Asumiendo que el SKU tiene un formato SKU:Quantity
                var parts = sku.Split(':');
                var skuCode = parts[0].Trim();
                var quantityText = parts.Length > 1 ? parts[1].Trim() : "0";

                var productVariant = skuToVariant.Where(x => x.Sku != null && x.Sku.Equals(skuCode, StringComparison.OrdinalIgnoreCase)).FirstOrDefault();
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

        foreach (var order in orders)
        {
            //await AddOrderAsync(order);
        }

        return ApiResponse<bool>.Success(true, null, "Órdenes creadas exitosamente");
    }
}
