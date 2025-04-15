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

        CreateMap<Warehouse, WarehouseDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name));

        CreateMap<Warehouse, WarehouseDetailDto>()
            .ForMember(dest => dest.CityName, opt => opt.MapFrom(src => src.City.Name))
            .ForMember(dest => dest.CountryName, opt => opt.MapFrom(src => src.Country.Name))
            .ForMember(dest => dest.Products, opt => opt.MapFrom(src =>
                src.Inventories
                .GroupBy(wi => wi.ProductVariant.Product)
                .Select(g => new ProductInventoryDto
                {
                    ShopifyId = g.Key.ShopifyProductId,
                    Id = g.Key.Id,
                    Name = g.Key.Name,
                    Variants = g.Select(vi => new VariantInventoryDto
                    {
                        ShopifyId = vi.ProductVariant.ShopifyVariantId,
                        Id = vi.ProductVariant.Id,
                        Name = !string.IsNullOrEmpty(vi.ProductVariant.Name) ? vi.ProductVariant.Name : string.Empty,
                        Quantity = vi.Quantity
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
            .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.Country.Name));

        // Filtros
        CreateMap<WarehouseFilterDto, WarehouseFilter>();
        CreateMap<OrderFilterDto, OrderFilter>();
        CreateMap<OrderStatusFilterDto, OrderStatusFilter>();
        CreateMap<OrderCategoryFilterDto, OrderCategoryFilter>();

        // DTOs de creación / actualización
        // CreateMap<WarehouseCreateDto, Warehouse>();
        // CreateMap<WarehouseUpdateDto, Warehouse>();
    }

}