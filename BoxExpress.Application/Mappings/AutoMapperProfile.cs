using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;

namespace BoxExpress.Application.Mappings;

public class AutoMapperProfile : Profile
{
    public AutoMapperProfile()
    {
        // Entity ➜ DTO para mostrar
        CreateMap<OrderStatus, OrderStatusDto>();
        CreateMap<OrderCategory, OrderCategoryDto>();

        CreateMap<WalletTransaction, WalletTransactionDto>()
            .ForMember(dest => dest.Store, opt => opt.MapFrom(src => src.Wallet.Store.Name))
            .ForMember(dest => dest.TransactionType, opt => opt.MapFrom(src => src.TransactionType.Name))
            .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.OrderStatus.Name))
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
                    }).ToList()
                }).ToList()
            ));


        CreateMap<Order, OrderDto>()
            .ForMember(dest => dest.ClientFullName, opt => opt.MapFrom(src => src.Client.FullName))
            .ForMember(dest => dest.ClientDocument, opt => opt.MapFrom(src => src.Client.Document))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.Name))
            .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category.Name))
            .ForMember(dest => dest.ClientPhone, opt => opt.MapFrom(src => src.Client.Phone))
            .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.ClientAddress, opt => opt.MapFrom(src => src.ClientAddress.Address))
            .ForMember(dest => dest.StoreName, opt => opt.MapFrom(src => src.Store.Name))
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.OrderCategoryId))
            .ForMember(dest => dest.StatusId, opt => opt.MapFrom(src => src.Status.Id))
            .ForMember(dest => dest.WarehouseName, opt => opt.MapFrom(src => src.Warehouse != null ? src.Warehouse.Name : string.Empty))
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name));

        CreateMap<ProductVariant, ProductVariantAutocompleteDto>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));

        // Filtros
        CreateMap<WarehouseFilterDto, WarehouseFilter>();
        CreateMap<OrderFilterDto, OrderFilter>();
        CreateMap<OrderStatusFilterDto, OrderStatusFilter>();
        CreateMap<OrderCategoryFilterDto, OrderCategoryFilter>();
        CreateMap<WalletTransactionFilterDto, WalletTransactionFilter>();

        // DTOs de creación / actualización
        // CreateMap<WarehouseCreateDto, Warehouse>();
        // CreateMap<WarehouseUpdateDto, Warehouse>();
    }

}