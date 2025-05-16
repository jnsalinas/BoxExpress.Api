using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Extensions;

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

        CreateMap<Warehouse, WarehouseDetailDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                src.Inventories
                .GroupBy(wi => wi.ProductVariant.Product)
                .Select(g => new ProductDto
                {
                    ShopifyId = g.Key.ShopifyProductId,
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Sku = g.Key.Sku,
                    Price = g.Key.Price,
                    Variants = g.Select(vi => new ProductVariantDto
                    {
                        ShopifyId = vi.ProductVariant.ShopifyVariantId,
                        Id = vi.ProductVariant.Id,
                        Name = !string.IsNullOrEmpty(vi.ProductVariant.Name) ? vi.ProductVariant.Name : string.Empty,
                        Quantity = vi.Quantity,
                        Sku = vi.ProductVariant.Sku,
                        Price = vi.ProductVariant.Price,
                        ReservedQuantity = vi.ReservedQuantity,
                        AvailableQuantity = vi.AvailableQuantity
                    }).ToList()
                }).ToList()
            ));

        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(src => src.Client.FullName))
            .ForMember(dest => dest.ClientDocument, opt => opt.MapFrom(src => src.Client.Document))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : string.Empty))
            .ForMember(dest => dest.ClientPhone, opt => opt.MapFrom(src => src.Client.Phone))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.ClientAddress, opt => opt.MapFrom(src => src.ClientAddress.Address))
            .ForMember(dest => dest.StoreId, opt => opt.MapFrom(src => src.StoreId))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.OrderCategoryId))
            .ForMember(dest => dest.TimeSlotStartTime, opt => opt.MapFrom(src => src.TimeSlot != null ? src.TimeSlot.StartTime : TimeSpan.Zero))
            .ForMember(dest => dest.TimeSlotEndTime, opt => opt.MapFrom(src => src.TimeSlot != null ? src.TimeSlot.EndTime : TimeSpan.Zero))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status.Id))
            .ForMember(dest => dest.CurrencyCode, opt => opt.MapFrom(src => src.Currency.Code))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name))
            .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems));

        CreateMap<ProductVariant, ProductVariantAutocompleteDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        CreateMap<WarehouseInventory, ProductVariantAutocompleteDto>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ProductVariant.Id))
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.ProductVariant.Name))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductVariant.Product.Name))
            .ForMember(dest => dest.AvailableUnits, opt => opt.MapFrom(src => src.AvailableQuantity));

        CreateMap<OrderStatusHistory, OrderStatusHistoryDto>()
            .ForMember(dest => dest.Creator, opt => opt.MapFrom(src => src.Creator.FirstName + " " + src.Creator.LastName))
            .ForMember(dest => dest.OldStatus, opt => opt.MapFrom(src => src.OldStatus.Name))
            .ForMember(dest => dest.NewStatus, opt => opt.MapFrom(src => src.NewStatus.Name));

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

        // Filtros
        CreateMap<WarehouseFilterDto, WarehouseFilter>();
        CreateMap<OrderFilterDto, OrderFilter>();
        CreateMap<OrderStatusFilterDto, OrderStatusFilter>();
        CreateMap<OrderCategoryFilterDto, OrderCategoryFilter>();
        CreateMap<WalletTransactionFilterDto, WalletTransactionFilter>();
        CreateMap<WarehouseInventoryTransferFilterDto, WarehouseInventoryTransferFilter>();
        CreateMap<WithdrawalRequestFilterDto, WithdrawalRequestFilter>();
        CreateMap<StoreFilterDto, StoreFilter>();

        // DTOs de creación / actualización
        CreateMap<CreateStoreDto, Store>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.StoreName));
        CreateMap<CreateStoreDto, User>();
        CreateMap<WithdrawalRequestCreateDto, WithdrawalRequest>();
        CreateMap<InventoryMovementDTO, InventoryMovement>();
        // CreateMap<WarehouseCreateDto, Warehouse>();
        // CreateMap<WarehouseUpdateDto, Warehouse>();
    }

}