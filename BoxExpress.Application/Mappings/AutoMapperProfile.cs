using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Extensions;
using BoxExpress.Application.Integrations.Shopify;
using BoxExpress.Utilities.Utils;

namespace BoxExpress.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Entity ➜ DTO para mostrar
        CreateMap<TimeSlot, TimeSlotDto>();
        CreateMap<OrderStatus, OrderStatusDto>();
        CreateMap<OrderCategory, OrderCategoryDto>();
        CreateMap<City, CityDto>();
        CreateMap<ClientAddress, ClientAddressDto>();
        CreateMap<Client, ClientDto>();
        CreateMap<Currency, CurrencyDto>();

        CreateMap<Store, StoreDto>()
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
            .ForMember(dest => dest.Balance, opt => opt.MapFrom(src => src.Wallet.Balance))
            .ForMember(dest => dest.PendingWithdrawals, opt => opt.MapFrom(src => src.Wallet.PendingWithdrawals))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.Name));

        CreateMap<WalletTransaction, WalletTransactionDto>()
            .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.Wallet.Store.Name))
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.Name))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus != null ? src.OrderStatus.Name : string.Empty))
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName));

        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name));




        //     public int Id { get; set; }
        //     public string Name { get; set; } = string.Empty;
        //     public string? ShopifyId { get; set; }
        //     public decimal? Price { get; set; }
        //     public string? Sku { get; set; }
        //     public List<ProductVariantDto> Variants { get; set; } = new();
        // public class ProductVariantDto
        // {
        //     public int Id { get; set; }
        //     public string Name { get; set; } = string.Empty;
        //     public int Quantity { get; set; }
        //     public string? ShopifyId { get; internal set; }
        //     public decimal? Price { get; set; }
        //     public string? Sku { get; set; }
        //     public int ReservedQuantity { get; set; }
        //     public int AvailableQuantity { get; set; }
        // }

        // CreateMap<WarehouseInventory, ProductDto>()
        //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
        //     .ForMember(dest => dest.ShopifyId, opt => opt.MapFrom(src => src.ProductVariant.Product.ShopifyProductId))
        //     .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductVariant.Product.Price))
        //     .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.ProductVariant.Product.Sku))
        //     .ForMember(dest => dest.Variants, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
        //     ;


        CreateMap<Warehouse, WarehouseDetailDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                src.Inventories
                .GroupBy(wi => wi.ProductVariant.Product)
                .Select(g => new ProductDto
                {
                    ShopifyProductId = g.Key.ShopifyProductId,
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Sku = g.Key.Sku,
                    Price = g.Key.Price,
                    Variants = g.Select(vi => new ProductVariantDto
                    {
                        ShopifyVariantId = vi.ProductVariant.ShopifyVariantId,
                        Id = vi.ProductVariant.Id,
                        Name = !string.IsNullOrEmpty(vi.ProductVariant.Name) ? vi.ProductVariant.Name : string.Empty,
                        Quantity = vi.Quantity,
                        Sku = vi.ProductVariant.Sku,
                        Price = vi.ProductVariant.Price,
                        ReservedQuantity = vi.ReservedQuantity,
                        AvailableQuantity = vi.AvailableQuantity,
                        PendingReturnQuantity = vi.PendingReturnQuantity
                    }).ToList()
                }).ToList()
            ));

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(src => src.Client.FirstName + " " + src.Client.LastName))
            .ForMember(dest => dest.ClientDocument, opt => opt.MapFrom(src => src.Client.Document))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.OrderCategoryId))
            .ForMember(dest => dest.ClientPhone, opt => opt.MapFrom(src => src.Client.Phone))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.ClientAddress, opt => opt.MapFrom(src => src.ClientAddress.Address))
            .ForMember(dest => dest.ClientAddressComplement, opt => opt.MapFrom(src => src.ClientAddress.Complement))
            .ForMember(dest => dest.ClientAddressPostalCode, opt => opt.MapFrom(src => src.ClientAddress.PostalCode))
            .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.TimeSlotStartTime, opt => opt.MapFrom(src => src.TimeSlot != null ? src.TimeSlot.StartTime : (TimeSpan?)null))
            .ForMember(dest => dest.TimeSlotEndTime, opt => opt.MapFrom(src => src.TimeSlot != null ? src.TimeSlot.EndTime : (TimeSpan?)null))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status.Id))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.Currency.Code))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Client));

        CreateMap<CreateOrderDto, Order>();
        CreateMap<CreateWarehouseDto, Warehouse>();
        CreateMap<OrderItemDto, OrderItem>();

        CreateMap<OrderSummary, OrderSummaryDto>();

        CreateMap<Product, ProductDto>();

        CreateMap<ProductVariant, ProductVariantDto>()
            .ForMember(dest => dest.OnTheWayQuantity, opt => opt.MapFrom(src => src.WarehouseInventories.Sum(w => w.OnTheWayQuantity)))
            .ForMember(dest => dest.ReservedQuantity, opt => opt.MapFrom(src => src.WarehouseInventories.Sum(w => w.ReservedQuantity)))
            .ForMember(dest => dest.PendingReturnQuantity, opt => opt.MapFrom(src => src.WarehouseInventories.Sum(w => w.PendingReturnQuantity)))
            .ForMember(dest => dest.AvailableQuantity, opt => opt.MapFrom(src => src.WarehouseInventories.Sum(w => w.AvailableQuantity)))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name))
            .ForMember(dest => dest.ProductSku, opt => opt.MapFrom(src => src.Product.Sku))
            .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.WarehouseInventories.Sum(w => w.Quantity)));

        CreateMap<ProductVariant, ProductVariantAutocompleteDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<WarehouseInventory, ProductVariantAutocompleteDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductVariant.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductVariant.Name))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store != null ? src.Store.Name : string.Empty))
            .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
            .ForMember(dest => dest.AvailableUnits, opt => opt.MapFrom(src => src.AvailableQuantity));

        CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName))
            .ForMember(dest => dest.OldStatus, opt => opt.MapFrom(src => src.OldStatus.Name))
            .ForMember(dest => dest.NewStatus, opt => opt.MapFrom(src => src.NewStatus.Name))
            .ForMember(dest => dest.DeliveryProviderName, opt => opt.MapFrom(src => src.DeliveryProvider != null ? src.DeliveryProvider.Name : string.Empty));

        CreateMap<OrderCategoryHistory, OrderCategoryHistoryDto>()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName))
            .ForMember(dest => dest.OldCategory, opt => opt.MapFrom(src => src.OldCategory.Name))
            .ForMember(dest => dest.NewCategory, opt => opt.MapFrom(src => src.NewCategory.Name));

        CreateMap<OrderItem, OrderItemDto>()
            .ForMember(dest => dest.ProductVariantId, opt => opt.MapFrom(src => src.ProductVariantId))
            .ForMember(dest => dest.ProductVariantName, opt => opt.MapFrom(src => src.ProductVariant.Name))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

        CreateMap<WarehouseInventoryTransferDto, WarehouseInventoryTransfer>()
            .ForMember(dest => dest.TransferDetails, opt => opt.MapFrom(src => src.TransferDetails));

        CreateMap<WarehouseInventoryTransferDetailDto, WarehouseInventoryTransferDetail>();

        CreateMap<WarehouseInventoryTransfer, WarehouseInventoryTransferDto>()
            .ForMember(dest => dest.StatusName, opt => opt.MapFrom(src => src.Status.ToSpanish()))
            .ForMember(dest => dest.TransferDetails, opt => opt.MapFrom(src => src.TransferDetails))
            .ForMember(dest => dest.ToWarehouse, opt => opt.MapFrom(src => src.ToWarehouse.Name))
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator.FullName))
            .ForMember(dest => dest.AcceptedBy, opt => opt.MapFrom(src => src.AcceptedByUser != null ? src.AcceptedByUser.FullName : string.Empty))
            .ForMember(dest => dest.FromWarehouse, opt => opt.MapFrom(src => src.FromWarehouse.Name));

        CreateMap<WarehouseInventoryTransferDetail, WarehouseInventoryTransferDetailDto>()
            .ForMember(dest => dest.ProductVariant, opt => opt.MapFrom(src => src.ProductVariant.Name))
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.ProductVariant.Product.Name));

        CreateMap<WithdrawalRequest, WithdrawalRequestDto>()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator.FullName))
            .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.Store.Name))
            .ReverseMap(); //todo: poner reversemap en otros

        CreateMap<Bank, BankDto>();
        CreateMap<DocumentType, DocumentTypeDto>();
        CreateMap<InventoryMovement, InventoryMovementDto>()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator != null ? src.Creator.FullName : string.Empty));
        CreateMap<WarehouseInventory, WarehouseInventoryDto>();
        CreateMap<Product, ProductDto>();
        CreateMap<InventoryHold, InventoryHoldDto>()
            .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(src => src.OrderItem != null ? src.OrderItem.Order.Client.FirstName + " " + src.OrderItem.Order.Client.LastName : string.Empty))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.WarehouseInventory != null && src.WarehouseInventory.Warehouse != null ? src.WarehouseInventory.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.WarehouseInventoryTransferId, opt => opt.MapFrom(src => src.WarehouseInventoryTransferDetail != null ? src.WarehouseInventoryTransferDetail.WarehouseInventoryTransferId : (int?)null))
            .ForMember(dest => dest.ProductLoanId, opt => opt.MapFrom(src => src.ProductLoanDetail != null ? src.ProductLoanDetail.ProductLoanId : (int?)null));

        // ProductLoan mappings
        CreateMap<ProductLoan, ProductLoanDto>()
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse.Name))
            .ForMember(dest => dest.CompanyName, opt => opt.MapFrom(src => src.Warehouse.Name))
            .ForMember(dest => dest.CreatedByName, opt => opt.MapFrom(src => src.CreatedBy.FullName))
            .ForMember(dest => dest.ProcessedByName, opt => opt.MapFrom(src => src.ProcessedBy != null ? src.ProcessedBy.FullName : string.Empty));

        CreateMap<ProductLoanDetail, ProductLoanDetailDto>()
            .ForMember(dest => dest.ProductVariantName, opt => opt.MapFrom(src => src.ProductVariant.Name ?? string.Empty))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
            .ForMember(dest => dest.ProductVariantDescription, opt => opt.MapFrom(src => src.ProductVariant.Name ?? string.Empty));

        CreateMap<WarehouseInventory, ProductVariantDto>()
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.warehouseId, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Id : (int?)null))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductVariant.Name ?? string.Empty))
            .ForMember(dest => dest.ShopifyVariantId, opt => opt.MapFrom(src => src.ProductVariant.ShopifyVariantId))
            .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.ProductVariant.Sku))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name));

        // Filtros
        CreateMap<WarehouseFilterDto, WarehouseFilter>();
        CreateMap<OrderFilterDto, OrderFilter>();
        CreateMap<OrderStatusFilterDto, OrderStatusFilter>();
        CreateMap<OrderCategoryFilterDto, OrderCategoryFilter>();
        CreateMap<WalletTransactionFilterDto, WalletTransactionFilter>();
        CreateMap<WarehouseInventoryTransferFilterDto, WarehouseInventoryTransferFilter>();
        CreateMap<WithdrawalRequestFilterDto, WithdrawalRequestFilter>();
        CreateMap<StoreFilterDto, StoreFilter>();
        CreateMap<WarehouseInventoryFilterDto, WarehouseInventoryFilter>();
        CreateMap<InventoryMovementFilterDto, InventoryMovementFilter>();
        CreateMap<InventoryHoldFilterDto, InventoryHoldFilter>();
        CreateMap<ProductVariantFilterDto, ProductVariantFilter>();
        CreateMap<ProductLoanFilterDto, ProductLoanFilter>();

        // DTOs de creación / actualización
        CreateMap<CreateStoreDto, Store>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StoreName));
        CreateMap<CreateStoreDto, User>();
        CreateMap<WithdrawalRequestCreateDto, WithdrawalRequest>();
        CreateMap<InventoryMovementDto, InventoryMovement>();
        // CreateMap<WarehouseCreateDto, Warehouse>();
        // CreateMap<WarehouseUpdateDto, Warehouse>();

        CreateMap<DeliveryProvider, DeliveryProviderDto>();
        CreateMap<DeliveryProviderFilterDto, DeliveryProviderFilter>();

        // Orden principal
        CreateMap<ShopifyOrderDto, Order>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Code, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Created_At))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Financial_Status))
            .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => decimal.Parse(src.Total_Price)))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Currency))
            // .ForMember(dest => dest.TrackingUrl, opt => opt.MapFrom(src => src.Order_Status_Url))
            // .ForMember(dest => dest.Tags, opt => opt.MapFrom(src => src.Tags))
            .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Note))
            .ForMember(dest => dest.Client, opt => opt.MapFrom(src => src.Customer))
            .ForMember(dest => dest.ClientAddress, opt => opt.MapFrom(src => src.Shipping_Address))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.Line_Items))
            // .ForMember(dest => dest.NoteAttributes, opt => opt.MapFrom(src => src.Note_Attributes))
            ;

        // Cliente
        CreateMap<ShopifyCustomerDto, Client>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.First_Name))
            .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Last_Name))
            .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
            .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone));

        // Dirección de envío
        CreateMap<ShopifyShippingAddressDto, ClientAddress>()
            // .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.First_Name))
            // .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Last_Name))
            .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address1))
            .ForMember(dest => dest.Address2, opt => opt.MapFrom(src => src.Address2))
            // .ForMember(dest => dest.Province, opt => opt.MapFrom(src => src.Province))
            .ForMember(dest => dest.Zip, opt => opt.MapFrom(src => src.Zip))
            .ForMember(dest => dest.Latitude, opt => opt.MapFrom(src => src.Latitude))
            .ForMember(dest => dest.Longitude, opt => opt.MapFrom(src => src.Longitude))
            // .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone))
            ;

        // Items de la orden
        CreateMap<ShopifyLineItemDto, OrderItem>()
            .ForMember(dest => dest.ExternalId, opt => opt.MapFrom(src => src.Id))
            // .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
            // .ForMember(dest => dest.Price, opt => opt.MapFrom(src => decimal.Parse(src.Price)))
            // .ForMember(dest => dest.Sku, opt => opt.MapFrom(src => src.Sku))
            // .ForMember(dest => dest.Variant, opt => opt.MapFrom(src => src.Variant_Title))
            // .ForMember(dest => dest.Discount, opt => opt.MapFrom(src => decimal.Parse(src.Total_Discount ?? "0")))
            ;

        // Note attributes
        // CreateMap<ShopifyNoteAttributeDto, OrderNoteAttribute>()
        //     .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
        //     .ForMember(dest => dest.Value, opt => opt.MapFrom(src => src.Value))
        //     .ForAllOtherMembers(opt => opt.Ignore());
    }

}