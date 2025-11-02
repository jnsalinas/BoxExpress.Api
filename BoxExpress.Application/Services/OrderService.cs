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
using BoxExpress.Application.Integrations.Shopify;
using Newtonsoft.Json;
using DocumentFormat.OpenXml.Office2010.Excel;

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
    private readonly IStoreRepository _storeRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IFileService _fileService;
    private readonly IWarehouseInventoryService _warehouseInventoryService;
    private readonly IWarehouseRepository _warehouseRepository;

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
        IUserContext userContext,
        IStoreRepository storeRepository,
        ICityRepository cityRepository,
        IFileService fileService,
        IWarehouseInventoryService warehouseInventoryService,
        IWarehouseRepository warehouseRepository

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
        _storeRepository = storeRepository;
        _cityRepository = cityRepository;
        _fileService = fileService;
        _warehouseInventoryService = warehouseInventoryService;
        _warehouseRepository = warehouseRepository;
    }

    public async Task<ApiResponse<IEnumerable<OrderDto>>> GetAllAsync(OrderFilterDto filter)
    {
        var (orders, totalCount) = await _repository.GetFilteredAsync(_mapper.Map<OrderFilter>(filter));
        var onTheWayStatus = await _orderStatusRepository.GetByNameAsync(OrderStatusConstants.OnTheWay);
        var canceledStatus = await _orderStatusRepository.GetByNameAsync(OrderStatusConstants.Cancelled);
        var ordersDto = _mapper.Map<List<OrderDto>>(orders);

        if (onTheWayStatus != null && canceledStatus != null && orders.Any(x => x.OrderStatusId == onTheWayStatus.Id || x.OrderStatusId == canceledStatus.Id))
        {
            var ordersIds = orders.Where(x => x.OrderStatusId == onTheWayStatus.Id).Select(x => x.Id).ToList();
            var ordersStatusHistories = await _orderStatusHistoryRepository.GetFilteredAsync(new OrderStatusHistoryFilter
            {
                OrderIds = ordersIds,
                NewStatusId = onTheWayStatus.Id,
            });

            var ordersCanceledIds = orders.Where(x => x.OrderStatusId == canceledStatus.Id).Select(x => x.Id).ToList();
            var orderCanceledCount = await _orderStatusHistoryRepository.GetOrderStatusCountByStatusesAsync(new OrderStatusHistoryFilter
            {
                OrderIds = ordersCanceledIds,
                NewStatusesId = new List<int> { canceledStatus.Id }
            });

            var ordersStatusHistoryOnTheWayAux = new OrderStatusHistory();
            var ordersStatusHistoryOnCanceledAux = new OrderStatusHistory();
            foreach (var order in ordersDto)
            {
                ordersStatusHistoryOnTheWayAux = ordersStatusHistories.OrderByDescending(x => x.CreatedAt).FirstOrDefault(x => x.OrderId == order.Id && x.NewStatusId == onTheWayStatus.Id);
                order.DeliveryProviderName = ordersStatusHistoryOnTheWayAux?.DeliveryProvider?.Name ?? string.Empty;
                order.DeliveryProviderId = ordersStatusHistoryOnTheWayAux?.DeliveryProviderId;
                order.CourierName = ordersStatusHistoryOnTheWayAux?.CourierName ?? string.Empty;

                //Cantidad de cancelaciones
                order.CanceledNotes = orderCanceledCount.FirstOrDefault(x => x.OrderId == order.Id)?.Notes ?? string.Empty;
                order.CanceledCount = orderCanceledCount.FirstOrDefault(x => x.OrderId == order.Id)?.Count ?? 0;
            }
        }

        // var (ordersPhones, totalCountPhones) = await _repository.GetFilteredAsync(new OrderFilter { Phones = ordersDto.Where(x => !string.IsNullOrEmpty(x.ClientPhone)).Select(x => x.ClientPhone).ToList(), IsAll = true });
        // foreach (var orderdto in ordersDto.Where(x => !string.IsNullOrEmpty(x.ClientPhone)))
        // {
        //     var ordersPhonesDuplicate = ordersPhones.Where(x => x.Client.Phone == orderdto.Client.Phone && x.Id != orderdto.Id).OrderByDescending(x => x.UpdatedAt);
        //     if (orderdto != null && ordersPhonesDuplicate.Count() > 1)
        //     {
        //         string notes = string.Empty;
        //         foreach (var orderPhone in ordersPhonesDuplicate)
        //         {
        //             notes += $"* Orden: {orderPhone.Id} \n Fecha: {orderPhone.CreatedAt.ToString("dd/MM/yyyy HH:mm")} \n Producto: {string.Join(",", orderPhone.OrderItems.Select(x => x.FullName ?? string.Empty))}\n\n";
        //         }
        //         orderdto.PhonesNotes = notes;
        //         orderdto.PhonesCount = ordersPhonesDuplicate.Count() + 1;
        //     }
        // }

        return ApiResponse<IEnumerable<OrderDto>>.Success(ordersDto, new PaginationDto(totalCount, filter.PageSize, filter.Page));
    }

    public async Task<ApiResponse<IEnumerable<OrderSummaryDto>>> GetSummaryAsync(OrderFilterDto filter)
    {
        var summary = await _repository.GetSummaryAsync(_mapper.Map<OrderFilter>(filter));
        return ApiResponse<IEnumerable<OrderSummaryDto>>.Success(_mapper.Map<List<OrderSummaryDto>>(summary));
    }

    public async Task<ApiResponse<IEnumerable<OrderSummaryDto>>> GetSummaryCategoryAsync(OrderFilterDto filter)
    {
        var summary = await _repository.GetSummaryCategoryAsync(_mapper.Map<OrderFilter>(filter));
        return ApiResponse<IEnumerable<OrderSummaryDto>>.Success(_mapper.Map<List<OrderSummaryDto>>(summary));
    }

    public async Task<ApiResponse<OrderDto?>> GetByIdAsync(int id) =>
        ApiResponse<OrderDto?>.Success(_mapper.Map<OrderDto>(await _repository.GetByIdAsync(id)));

    public async Task<ApiResponse<OrderDto>> UpdateWarehouseAsync(int orderId, UpdateWarehouseRequestDto dto)
    {
        Order? order = await _repository.GetByIdWithDetailsAsync(orderId);
        if (order == null)
            return ApiResponse<OrderDto>.Fail("Order not found");

        //log
        await _orderCategoryHistoryRepository.AddAsync(new()
        {
            CreatedAt = DateTime.UtcNow,
            OrderId = order.Id,
            OldCategoryId = order.OrderCategoryId,
            NewCategoryId = dto.CategoryId,
            CreatorId = _userContext.UserId.Value,
        });


        bool isReset = false;
        OrderCategory? orderCategory = null;
        if (dto.CategoryId != null)
        {
            orderCategory = await _orderCategoryRepository.GetByIdAsync(dto.CategoryId.Value);
            if (dto.CategoryId != null && orderCategory != null && (orderCategory.Name == OrderCategoryConstants.WithoutCoverage || orderCategory.Name == OrderCategoryConstants.RepeatedOrder))
            {
                order.OrderStatusId = null;
                order.Status = null;
            }
            else
                isReset = true;
        }
        else
            isReset = true;

        if (isReset)
        {
            var orderStatusUnscheduled = await _orderStatusRepository.GetByNameAsync(OrderStatusConstants.Unscheduled);
            order.OrderStatusId = orderStatusUnscheduled.Id;
            order.Status = orderStatusUnscheduled;
        }

        Warehouse? warehouse = null;
        if (dto.WarehouseId != null)
            warehouse = await _warehouseRepository.GetByIdAsync(dto.WarehouseId.Value);
        else
            order.WarehouseId = null;

        order.Warehouse = warehouse;
        order.OrderCategoryId = dto.CategoryId;
        order.Category = orderCategory;
        order.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<OrderDto>> UpdateStatusAsync(int orderId, int statusId, ChangeStatusDto? changeStatusDto)
    {
        #region validations 
        Order? order = await _repository.GetByIdWithDetailsAsync(orderId);
        if (order == null)
            return ApiResponse<OrderDto>.Fail("Order not found");
        var previousStatus = order.Status;

        if (order.OrderStatusId.Equals(statusId))
            return ApiResponse<OrderDto>.Fail("Same status");

        OrderStatus? orderStatus = await _orderStatusRepository.GetByIdAsync(statusId);
        if (orderStatus == null)
            return ApiResponse<OrderDto>.Fail("Status not found");

        #endregion

        if (orderStatus.Name == OrderStatusConstants.Delivered)
        {
            //todo: validar si la orden estaba devuelta y tiene items por devolucion que tome el ultimo y lo revierta supongo
            await _inventoryMovementService.ProcessDeliveryAsync(order);
            await _walletTransactionService.RegisterSuccessfulDeliveryAsync(order, statusId);
        }
        else
        {
            bool isCanceled = orderStatus.Name.Equals(OrderStatusConstants.Cancelled) || orderStatus.Name.Equals(OrderStatusConstants.CancelledAlt) && order.WarehouseId.HasValue;

            if (previousStatus.Name.Equals(OrderStatusConstants.Delivered))
            {
                await _inventoryMovementService.RevertDeliveryAsync(order);
                await _walletTransactionService.RegisterStatusCorrectionAsync(order, statusId);

                //vuelve a reservar del inventario para luego validar si es cancelado y reusar lo mismo cuando previamente no tiene el estado de entregado
                if (isCanceled)
                {
                    var ReserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId!.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.Active);
                    if (!ReserveInventory.IsSuccess)
                        return ApiResponse<OrderDto>.Fail(ReserveInventory.Message ?? "Inventory not available");

                    await _warehouseInventoryService.ManageOnTheWayInventoryAsync(order.WarehouseId!.Value, order.OrderItems);
                }
            }
            //si la orden estaba en programado y pasa a sin programar, se libera el hold
            else if (previousStatus.Name.Equals(OrderStatusConstants.Scheduled) && orderStatus.Name.Equals(OrderStatusConstants.Unscheduled))
            {
                var ReverseInventory = await _inventoryHoldService.ReverseInventoryHoldAsync(order.WarehouseId!.Value, order.OrderItems);
                if (!ReverseInventory.IsSuccess)
                    return ApiResponse<OrderDto>.Fail(ReverseInventory.Message ?? "Inventory not available");
            }
            //si la orden es progrmaada debe validar inventario y crear el hold en active
            else if (orderStatus.Name.Equals(OrderStatusConstants.Scheduled))
            {
                var ReserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId!.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.Active);
                if (!ReserveInventory.IsSuccess)
                    return ApiResponse<OrderDto>.Fail(ReserveInventory.Message ?? "Inventory not available");
            }
            //se cambio a Scheduled la logica de hold
            else if (orderStatus.Name.Equals(OrderStatusConstants.OnTheWay))
            {
                await _warehouseInventoryService.ManageOnTheWayInventoryAsync(order.WarehouseId!.Value, order.OrderItems);
                // var ReserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId!.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.Active);
                // if (!ReserveInventory.IsSuccess)
                //     return ApiResponse<OrderDto>.Fail(ReserveInventory.Message ?? "Inventory not available");
            }
            
            //si la orden es cancelada y tiene bodega asignada, se reserva el hold en PendingReturn el inventario
            if (isCanceled)
            {
                var reserveInventory = await _inventoryHoldService.HoldInventoryForOrderAsync(order.WarehouseId.Value, order.OrderItems, Domain.Enums.InventoryHoldStatus.PendingReturn);
                if (!reserveInventory.IsSuccess)
                    return ApiResponse<OrderDto>.Fail(reserveInventory.Message ?? "Inventory not available");
            }
        }

        string? photoUrl = null;
        if (changeStatusDto != null && changeStatusDto.Photo != null)
        {
            photoUrl = await _fileService.UploadFileAsync(changeStatusDto.Photo);
        }

        await _orderStatusHistoryRepository.AddAsync(new()
        {
            OrderId = order.Id,
            OldStatusId = order.OrderStatusId,
            NewStatusId = statusId,
            CreatedAt = DateTime.UtcNow,
            CreatorId = _userContext.UserId != null ? _userContext.UserId.Value : 1,
            CourierName = changeStatusDto?.CourierName,
            DeliveryProviderId = changeStatusDto?.DeliveryProviderId,
            OnRouteEvidenceUrl = photoUrl,
            Notes = changeStatusDto?.Comments,
        });

        order.Status = orderStatus;
        order.OrderStatusId = statusId;
        order.UpdatedAt = DateTime.UtcNow;

        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<OrderDto>> UpdateScheduleAsync(int orderId, OrderScheduleUpdateDto orderScheduleUpdateDto)
    {
        if (orderScheduleUpdateDto.StatusId.HasValue)
        {
            await UpdateStatusAsync(orderId, orderScheduleUpdateDto.StatusId.Value, null);
        }

        Order? order = await _repository.GetByIdWithDetailsAsync(orderId);
        if (order == null)
        {
            return ApiResponse<OrderDto>.Fail("Order not found");
        }

        order.ScheduledDate = orderScheduleUpdateDto.ScheduledDate ?? order.ScheduledDate;
        order.TimeSlotId = orderScheduleUpdateDto.TimeSlotId;
        await _repository.UpdateAsync(order);
        return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
    }

    public async Task<ApiResponse<List<OrderStatusHistoryDto>>> GetStatusHistoryAsync(int orderId)
    {
        var listOrderStatusHistory = await _orderStatusHistoryRepository.GetByOrderIdAsync(orderId);
        List<OrderStatusHistoryDto> listHistory = new List<OrderStatusHistoryDto>();
        foreach (var item in listOrderStatusHistory)
        {
            var historyDto = _mapper.Map<OrderStatusHistoryDto>(item);
            if (!string.IsNullOrEmpty(item.OnRouteEvidenceUrl))
            {
                var fileResult = await _fileService.GetTempUrlAsync(item.OnRouteEvidenceUrl, TimeSpan.FromMinutes(30));
                if (!string.IsNullOrEmpty(fileResult))
                {
                    historyDto.PhotoUrl = fileResult;
                }
            }
            listHistory.Add(historyDto);
        }
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

    public async Task<ApiResponse<OrderDto>> AddOrderMockAsync(string objShopifyOrderDto)
    {
        var createOrderDto = new CreateOrderDto
        {
            StoreId = 1,
            ClientFirstName = "Juan",
            ClientLastName = "Perez",
            ClientEmail = "juan.perez@gmail.com",
            ClientPhone = "1234567890",
            ClientAddress = "Calle 123",
            ClientAddressComplement = "Apt 123",
            CityId = 1,
            PostalCode = "12345",
            Latitude = 19.432607,
            Longitude = -99.133209,
            TotalAmount = 100,
            CurrencyId = 1,
            Notes = "Orden creada desde mock:",
            Contains = objShopifyOrderDto
        };

        return await AddOrderAsync(createOrderDto);
    }

    public async Task<ApiResponse<OrderDto>> AddOrderAsync(ShopifyOrderDto shopifyOrderDto)
    {
        int storeId = shopifyOrderDto.StoreId ?? 0;
        if (shopifyOrderDto.StoreId == null && !string.IsNullOrEmpty(shopifyOrderDto.Store_Domain))
        {
            var (stores, totalCount) = await _storeRepository.GetFilteredAsync(new StoreFilter { ShopifyShopDomain = shopifyOrderDto.Store_Domain });
            if (stores.Count == 0)
            {
                return ApiResponse<OrderDto>.Fail("Store not found");
            }

            storeId = stores.First().Id;
        }
        else if (shopifyOrderDto.PublicId.HasValue)
        {
            var (stores, totalCount) = await _storeRepository.GetFilteredAsync(new StoreFilter { PublicId = shopifyOrderDto.PublicId });
            if (stores.Count == 0)
            {
                return ApiResponse<OrderDto>.Fail("Store not found");
            }
            storeId = stores.First().Id;
        }

        var warehouseInventoroies = await _warehouseInventoryRepository.GetBySkusAsync(shopifyOrderDto.Line_Items.Select(x => x.Sku).ToList().ToHashSet(), storeId);
        var contains = string.Empty; //shopifyOrderDto.Order_Status_Url 
        foreach (var item in shopifyOrderDto.Line_Items)
        {
            var warehouseInventory = warehouseInventoroies.FirstOrDefault(x => x.ProductVariant.Sku == item.Sku);
            if (warehouseInventory != null)
            {
                item.ProductVariantId = warehouseInventory.ProductVariantId;
            }

            contains += $"Producto: {item.Title} - SKU: {item.Sku} - Cantidad: {item.Quantity};";
        }

        int? cityId = null;
        if (!string.IsNullOrEmpty(shopifyOrderDto.Shipping_Address.City))
        {
            var city = await _cityRepository.GetByNameAsync(shopifyOrderDto.Shipping_Address.City);
            cityId = city?.Id;
        }
        string colonia = shopifyOrderDto.Note_Attributes?.FirstOrDefault(x => x.Name.ToLower().Trim() == "nombre de la calle")?.Value;
        string scheduleDate = shopifyOrderDto.Note_Attributes?.FirstOrDefault(x => x.Name.ToLower().Trim() == "colocar día y entre que horario puede recibir")?.Value;

        //todo agregar a notas Colocar día y entre que horario puede recibir
        var createOrderDto = new CreateOrderDto
        {
            StoreId = storeId,
            ClientFirstName = shopifyOrderDto.Shipping_Address.First_Name,
            ClientLastName = shopifyOrderDto.Shipping_Address.Last_Name,
            ClientEmail = shopifyOrderDto.Email,
            ClientPhone = shopifyOrderDto.Shipping_Address.Phone,
            ClientAddress = shopifyOrderDto.Shipping_Address.City + ", " + (colonia != null ? colonia + ", " : "") + shopifyOrderDto.Shipping_Address.Address1,
            ClientAddressComplement = shopifyOrderDto.Shipping_Address.Address2,
            CityId = cityId,
            PostalCode = shopifyOrderDto.Shipping_Address.Zip,
            Latitude = shopifyOrderDto.Shipping_Address.Latitude,
            Longitude = shopifyOrderDto.Shipping_Address.Longitude,
            OrderItems = shopifyOrderDto.Line_Items.Select(x => new OrderItemDto
            {
                ProductVariantId = x.ProductVariantId ?? 0,
                Quantity = x.Quantity
            }).ToList(),
            Code = shopifyOrderDto.Id.ToString() + "-" + shopifyOrderDto.Name,
            TotalAmount = decimal.Parse(shopifyOrderDto.Total_Price),
            CurrencyId = 1,
            Notes = "Orden creada desde Shopify: " + shopifyOrderDto.Note + (scheduleDate != null ? " - " + scheduleDate : ""),
            CreatedAt = shopifyOrderDto.Created_At,
            Contains = contains
        };

        return await AddOrderAsync(createOrderDto);
    }

    public async Task<ApiResponse<OrderDto>> AddOrderAsync(CreateOrderDto createOrderDto)
    {
        try
        {
            int? userId = _userContext.UserId;
            await _unitOfWork.BeginTransactionAsync();

            var createdAt = createOrderDto.CreatedAt ?? DateTime.UtcNow;

            // Create new client
            var client = new Client
            {
                FirstName = createOrderDto.ClientFirstName,
                LastName = createOrderDto.ClientLastName,
                Email = createOrderDto.ClientEmail,
                CreatedAt = createdAt,
                Phone = createOrderDto.ClientPhone,
            };
            await _unitOfWork.Clients.AddAsync(client);

            // Create client address
            var clientAddress = new ClientAddress
            {
                Client = client,  // ← Referencia de navegación en lugar de ClientId
                Address = createOrderDto.ClientAddress,
                Complement = createOrderDto.ClientAddressComplement,
                CityId = createOrderDto.CityId,
                Latitude = createOrderDto.Latitude,
                Longitude = createOrderDto.Longitude,
                IsDefault = true,
                CreatedAt = createdAt,
                PostalCode = createOrderDto.PostalCode,
            };
            await _unitOfWork.ClientAddresses.AddAsync(clientAddress);

            // Get default order status
            var defaultOrderStatus = await _orderStatusRepository.GetByNameAsync(OrderStatusConstants.Unscheduled);
            if (defaultOrderStatus == null)
            {
                await _unitOfWork.RollbackAsync();
                return ApiResponse<OrderDto>.Fail("Estado de orden por defecto no encontrado");
            }

            //Check if order code already exists
            // if (!string.IsNullOrEmpty(createOrderDto.Code))
            // {
            //     var orderExist = await _repository.GetByCodeAsync(createOrderDto.Code, createOrderDto.StoreId);
            //     if (orderExist != null)
            //     {
            //         await _unitOfWork.RollbackAsync();
            //         return ApiResponse<OrderDto>.Fail($"Orden {orderExist.Code} ya existe", _mapper.Map<OrderDto>(orderExist));
            //     }
            // }

            var store = await _storeRepository.GetByIdAsync(createOrderDto.StoreId);
            if (store == null)
            {
                return ApiResponse<OrderDto>.Fail("Store not found");
            }

            decimal deliveryFee = store.DeliveryFee ?? 0;
            if (deliveryFee == 0)
            {
                var countryCode = "MX"; //todo cambiar a la ciudad de la direccion
                var deliveryFeeSection = _configuration.GetSection($"{countryCode}:DeliveryFee");
                if (deliveryFeeSection.Exists() && decimal.TryParse(deliveryFeeSection.Value, out var fee))
                {
                    deliveryFee = fee;
                }
                else
                {
                    deliveryFee = 150;
                }
            }

            // Create Order
            var order = new Order
            {
                StoreId = createOrderDto.StoreId,
                OrderStatusId = defaultOrderStatus.Id,
                DeliveryFee = deliveryFee,
                CurrencyId = createOrderDto.CurrencyId,
                Client = client,
                ClientAddress = clientAddress,
                CityId = createOrderDto.CityId,
                Code = createOrderDto.Code,
                TotalAmount = createOrderDto.TotalAmount,
                Notes = createOrderDto.Notes,
                CreatedAt = createdAt,
                CreatorId = userId,
                IsEnabled = true,
                Contains = createOrderDto.Contains,
            };
            await _unitOfWork.Orders.AddAsync(order);

            // Get product variants
            var productVariants = await _productVariantRepository.GetByIdsAsync(
                createOrderDto.OrderItems.Select(x => x.ProductVariantId).ToList());

            // Create OrderItems usando referencias de navegación
            foreach (var orderItemDto in createOrderDto.OrderItems)
            {
                if (orderItemDto.ProductVariantId > 0)
                {
                    var orderItem = new OrderItem
                    {
                        Order = order,  // ← Referencia de navegación en lugar de OrderId
                        ProductVariantId = orderItemDto.ProductVariantId,
                        Quantity = orderItemDto.Quantity ?? 0,
                        CreatedAt = createdAt,
                        UnitPrice = productVariants.FirstOrDefault(x => x.Id == orderItemDto.ProductVariantId)?.Price ?? 0,
                    };
                    await _unitOfWork.OrderItems.AddAsync(orderItem);
                }

            }

            // Create order status history
            var orderStatusHistory = new OrderStatusHistory
            {
                CreatedAt = createdAt,
                Order = order,  // ← Referencia de navegación
                CreatorId = userId,
                NewStatusId = defaultOrderStatus.Id
            };
            await _unitOfWork.OrderStatusHistories.AddAsync(orderStatusHistory);

            // SOLO UN SaveChangesAsync al final - TODO se guarda de una vez
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order), null, "Orden creada exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            await AddOrderMockAsync(JsonConvert.SerializeObject(createOrderDto) + "|" + JsonConvert.SerializeObject(ex));
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
            var skuCell = ordersSheet.Cell(row, 14).GetString();
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
                StoreId = dto.StoreId,
                ClientFirstName = ordersSheet.Cell(row, col++).GetString(),
                ClientLastName = ordersSheet.Cell(row, col++).GetString(),
                ClientPhone = ordersSheet.Cell(row, col++).GetString(),
                ClientEmail = ordersSheet.Cell(row, col++).GetString(),
                ClientAddress = ordersSheet.Cell(row, col++).GetString(),
                ClientAddressComplement = ordersSheet.Cell(row, col++).GetString(),
                CityId = int.Parse(ordersSheet.Cell(row, col++).GetString()),
                PostalCode = ordersSheet.Cell(row, col++).GetString(),
                Latitude = double.TryParse(ordersSheet.Cell(row, col++).GetString(), out var lat) ? lat : null,
                Longitude = double.TryParse(ordersSheet.Cell(row, col++).GetString(), out var lon) ? lon : null,
                Code = ordersSheet.Cell(row, col++).GetString(),
                TotalAmount = decimal.Parse(ordersSheet.Cell(row, col++).GetString()),
                Notes = ordersSheet.Cell(row, col++).GetString(),
                OrderItems = new List<OrderItemDto>()
            };

            order.CurrencyId = 1;

            var skuvalidations = string.Empty;
            var orderItemsSKUS = ordersSheet.Cell(row, col++).GetString().Split(';');
            foreach (var sku in orderItemsSKUS)
            {
                var parts = sku.Split(':');
                var skuCode = parts[0].Trim();
                var quantityText = parts.Length > 1 ? parts[1].Trim() : "0";

                var warehouseInventoryId = skuToVariant
                    .FirstOrDefault(x => x.ProductVariant != null && x.ProductVariant.Sku != null && x.ProductVariant.Sku.Equals(skuCode, StringComparison.OrdinalIgnoreCase));
                if (warehouseInventoryId != null)
                {
                    order.OrderItems.Add(new OrderItemDto
                    {
                        ProductVariantId = warehouseInventoryId.ProductVariantId,
                        Quantity = int.Parse(quantityText)
                    });
                }
                else
                {
                    skuvalidations += $"El SKU {skuCode} no existe en el inventario";
                }
            }

            order.RowBulkUpload = row;
            order.ResultBulkUpload = string.IsNullOrEmpty(skuvalidations) ? "OK" : skuvalidations;
            orders.Add(order);
        }

        var result = new List<OrderExcelUploadResponseDto>();
        foreach (var order in orders)
        {
            var response = await AddOrderAsync(order);

            var resultMessage = response.Message ?? "";
            resultMessage += (!string.IsNullOrEmpty(resultMessage) ? " " : string.Empty) + (order.ResultBulkUpload ?? "");
            result.Add(new OrderExcelUploadResponseDto
            {
                Id = response.Data?.Id,
                RowNumber = order.RowBulkUpload,
                Code = order.Code ?? string.Empty,
                Message = resultMessage,
                IsLoaded = response.IsSuccess
            });
        }

        return ApiResponse<List<OrderExcelUploadResponseDto>>.Success(result, null, "Órdenes creadas exitosamente");
    }

    public async Task<ApiResponse<OrderDto>> UpdateOrderAsync(int id, CreateOrderDto createOrderDto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var order = await _repository.GetByIdWithDetailsAsync(id);
            if (order == null)
                return ApiResponse<OrderDto>.Fail("Order not found");


            var deliveredStatus = await _orderStatusRepository.GetByNameAsync(OrderStatusConstants.Delivered);
            if (order.OrderStatusId == deliveredStatus?.Id)
                return ApiResponse<OrderDto>.Fail("No se puede actualizar una orden entregada");

            order.Client.FirstName = createOrderDto.ClientFirstName;
            order.Client.LastName = createOrderDto.ClientLastName;
            order.Client.Email = createOrderDto.ClientEmail;
            order.Client.Phone = createOrderDto.ClientPhone;
            order.ClientAddress.Address = createOrderDto.ClientAddress;
            order.ClientAddress.Complement = createOrderDto.ClientAddressComplement;
            order.ClientAddress.Latitude = createOrderDto.Latitude;
            order.ClientAddress.Longitude = createOrderDto.Longitude;
            order.ClientAddress.CityId = createOrderDto.CityId;
            order.ClientAddress.PostalCode = createOrderDto.PostalCode;
            order.ClientAddress.IsDefault = true;
            order.ClientAddress.UpdatedAt = DateTime.UtcNow;
            order.Client.UpdatedAt = DateTime.UtcNow;

            order.TotalAmount = createOrderDto.TotalAmount;
            order.Notes = createOrderDto.Notes;
            order.UpdatedAt = DateTime.UtcNow;
            order.DeliveryFee = createOrderDto.DeliveryFee ?? 0;
            order.CurrencyId = createOrderDto.CurrencyId;
            order.Code = createOrderDto.Code;
            order.StoreId = createOrderDto.StoreId;

            // Obtener IDs de items que vienen en el DTO
            var dtoItemIds = createOrderDto.OrderItems
                .Where(x => x.Id.HasValue)
                .Select(x => x.Id.Value)
                .ToHashSet();

            // Eliminar items que ya no están en el DTO
            var itemsToRemove = order.OrderItems
                .Where(x => !dtoItemIds.Contains(x.Id))
                .ToList();

            foreach (var item in itemsToRemove)
            {
                await _unitOfWork.OrderItems.DeleteAsync(item.Id);
            }

            // Actualizar items existentes
            foreach (var orderItem in createOrderDto.OrderItems.Where(x => x.Id.HasValue))
            {
                var currentOrderItem = order.OrderItems.FirstOrDefault(x => x.Id == orderItem.Id);
                if (currentOrderItem != null && orderItem.Quantity.HasValue)
                {
                    var hasChanges = currentOrderItem.Quantity != orderItem.Quantity.Value ||
                                   currentOrderItem.ProductVariantId != orderItem.ProductVariantId;

                    if (hasChanges)
                    {
                        currentOrderItem.ProductVariantId = orderItem.ProductVariantId;
                        currentOrderItem.Quantity = orderItem.Quantity.Value;
                        currentOrderItem.UpdatedAt = DateTime.UtcNow;
                        await _unitOfWork.OrderItems.UpdateAsync(currentOrderItem);
                    }
                }
            }

            // Agregar nuevos items
            foreach (var orderItem in createOrderDto.OrderItems.Where(x => !x.Id.HasValue))
            {
                if (orderItem.Quantity.HasValue)
                {
                    var newOrderItem = new OrderItem
                    {
                        OrderId = order.Id,
                        ProductVariantId = orderItem.ProductVariantId,
                        Quantity = orderItem.Quantity.Value,
                        CreatedAt = DateTime.UtcNow,
                    };
                    await _unitOfWork.OrderItems.AddAsync(newOrderItem);
                }
            }

            order.UpdatedAt = DateTime.UtcNow;
            order.CreatorId = _userContext.UserId.Value;

            await _unitOfWork.ClientAddresses.UpdateAsync(order.ClientAddress);
            await _unitOfWork.Clients.UpdateAsync(order.Client);
            await _unitOfWork.Orders.UpdateAsync(order);

            // IMPORTANTE: Guardar cambios antes del commit
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            return ApiResponse<OrderDto>.Success(_mapper.Map<OrderDto>(order));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<OrderDto>.Fail("Error al actualizar Orden: " + ex.Message);
        }
    }

    public async Task<ApiResponse<List<OrderExcelUploadResponseDto>>> BulkChangeStatusAsync(BulkChangeOrdersStatusDto bulkChangeStatusDto)
    {
        //todo si es ruta se va a mensajero propio 
        var results = new List<OrderExcelUploadResponseDto>();
        foreach (var orderId in bulkChangeStatusDto.OrderIds)
        {
            var result = await UpdateStatusAsync(orderId, bulkChangeStatusDto.StatusId, bulkChangeStatusDto);
            if (!result.IsSuccess)
            {
                results.Add(new OrderExcelUploadResponseDto
                {
                    Id = orderId,
                    Message = "Error al actualizar Orden" + " " + orderId + " " + (result.Message ?? ""),
                    IsLoaded = false
                });
            }
            else
            {
                results.Add(new OrderExcelUploadResponseDto
                {
                    Id = orderId,
                    Message = "Orden actualizada exitosamente" + " " + orderId,
                    IsLoaded = true
                });
            }
        }
        return ApiResponse<List<OrderExcelUploadResponseDto>>.Success(results, null, results.Any(x => !x.IsLoaded) ? "Error al actualizar Ordenes" : "Ordenes actualizadas exitosamente");
    }
}