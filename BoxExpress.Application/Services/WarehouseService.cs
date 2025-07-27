using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Interfaces;
using BoxExpress.Application.Dtos;
using BoxExpress.Domain.Filters;
using AutoMapper;
using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Enums;
using BoxExpress.Domain.Constants;
using BoxExpress.Utilities;

namespace BoxExpress.Application.Services;

public class WarehouseService : IWarehouseService
{
    private readonly IWarehouseInventoryTransferRepository _warehouseInventoryTransferRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IWarehouseRepository _repository;
    private readonly IMapper _mapper;
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly ICityRepository _cityRepository;
    private readonly IWarehouseInventoryService _warehouseInventoryService;
    private readonly IUserContext _userContext;
    public WarehouseService(
        IWarehouseInventoryTransferRepository warehouseInventoryTransferRepository,
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IWarehouseRepository repository,
        IMapper mapper,
        IUnitOfWork unitOfWork,
        IRoleRepository roleRepository,
        ICityRepository cityRepository,
        IWarehouseInventoryService warehouseInventoryService,
        IUserContext userContext)
    {
        _warehouseInventoryTransferRepository = warehouseInventoryTransferRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _roleRepository = roleRepository;
        _cityRepository = cityRepository;
        _warehouseInventoryService = warehouseInventoryService;
        _userContext = userContext;
    }

    public async Task<ApiResponse<IEnumerable<WarehouseDto>>> GetAllAsync(WarehouseFilterDto filter) =>
         ApiResponse<IEnumerable<WarehouseDto>>.Success(_mapper.Map<List<WarehouseDto>>(await _repository.GetFilteredAsync(_mapper.Map<WarehouseFilter>(filter))));

    public async Task<ApiResponse<WarehouseDetailDto?>> GetByIdAsync(int id) =>
        ApiResponse<WarehouseDetailDto?>.Success(_mapper.Map<WarehouseDetailDto>(await _repository.GetByIdWithDetailsAsync(id)));

    // UnitOfWork es un patrón que coordina varios repositorios para que puedas hacer múltiples operaciones sobre la base de datos en una única transacción.
    // ✅ Su objetivo es garantizar que todo se guarde o nada se guarde (atomicidad).
    public async Task<ApiResponse<bool>> AddInventoryToWarehouseAsync(int warehouseId, List<CreateProductWithVariantsDto> products)
    {
        string errorMessage = string.Empty;
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            foreach (var productDto in products)
            {
                Product? product = null;
                if (productDto.Id > 0)
                {
                    product = await _unitOfWork.Products.GetByIdAsync(productDto.Id.Value);
                    if (product == null)
                        return ApiResponse<bool>.Fail("Producto no encontrado");

                    product.Name = productDto.Name;
                    product.Sku = productDto.Sku;
                    product.Price = productDto.Price;
                    product.Quantity = productDto.Quantity;
                    await _unitOfWork.Products.UpdateAsync(product);
                }
                else
                {
                    product = new()
                    {
                        CreatedAt = DateTime.UtcNow,
                        Name = productDto.Name,
                        Sku = productDto.Sku,
                        Price = productDto.Price
                    };
                    await _unitOfWork.Products.AddAsync(product);
                }

                foreach (var variantDto in productDto.Variants)
                {
                    ProductVariant? productVariant = null;
                    if (variantDto.Id > 0)
                    {
                        await _warehouseInventoryService.UpdateAsync(variantDto.Id.Value, new UpdateWarehouseInventoryDto
                        {
                            ShopifyVariantId = variantDto.ShopifyId,
                            VariantName = variantDto.Name,
                            VariantSku = variantDto.Sku,
                            Price = variantDto.Price,
                            Quantity = variantDto.Quantity,
                            StoreId = variantDto.StoreId
                        });
                        productVariant = await _unitOfWork.Variants.GetByIdAsync(variantDto.Id.Value);
                    }
                    else
                    {
                        var productVariantSkuExist = await _warehouseInventoryRepository.GetBySkusAsync(new HashSet<string> { variantDto.Sku }, variantDto.StoreId);
                        if (productVariantSkuExist.Any())
                        {
                            errorMessage += $"{variantDto.Sku} ya existe";
                        }
                        else
                        {
                            productVariant = new()
                            {
                                CreatedAt = DateTime.UtcNow,
                                Name = variantDto.Name,
                                ShopifyVariantId = variantDto.ShopifyId,
                                Product = product,
                                Sku = variantDto.Sku,
                                Price = variantDto.Price
                            };
                            await _unitOfWork.Variants.AddAsync(productVariant);

                            await _unitOfWork.Inventories.AddAsync(new()
                            {
                                CreatedAt = DateTime.UtcNow,
                                WarehouseId = warehouseId,
                                ProductVariant = productVariant,
                                Quantity = variantDto.Quantity,
                                StoreId = variantDto.StoreId
                            });

                            await _unitOfWork.InventoryMovements.AddAsync(new InventoryMovement
                            {
                                CreatorId = _userContext.UserId,
                                CreatedAt = DateTime.UtcNow,
                                WarehouseId = warehouseId,
                                ProductVariant = productVariant,
                                Quantity = variantDto.Quantity,
                                MovementType = InventoryMovementType.InitialStock,
                                Notes = "Inventario inicial",
                                Reference = $"Initial-Stock-{product.Name}-{productVariant.Name}"
                            });
                        }
                    }
                }
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();

            if (errorMessage.Length > 0)
            {
                return ApiResponse<bool>.Fail("Algunos SKUs ya existen: " + errorMessage);
            }

            return ApiResponse<bool>.Success(true, null, "Inventario creado exitosamente");
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<bool>.Fail("Error al guardar el inventario: " + ex.Message);
        }
    }

    public async Task<ApiResponse<bool>> CreateAsync(CreateWarehouseDto createWarehouseDto)
    {
        Role? role = await _roleRepository.GetByNameAsync(RolConstants.Warehouse);
        if (role == null)
            return ApiResponse<bool>.Fail("Rol de bodega no encontrado");

        City? city = await _cityRepository.GetByIdAsync(createWarehouseDto.CityId);
        if (city == null)
            return ApiResponse<bool>.Fail("Ciudad no encontrada");

        var existingWarehouse = await _repository.GetFilteredAsync(new WarehouseFilter
        {
            Name = createWarehouseDto.Name,
        });

        if (existingWarehouse.Any())
            return ApiResponse<bool>.Fail("Ya existe un almacén con ese nombre");

        await _unitOfWork.BeginTransactionAsync();
        Warehouse warehouse = _mapper.Map<Warehouse>(createWarehouseDto);
        warehouse.CreatedAt = DateTime.UtcNow;
        warehouse.CountryId = city.CountryId;

        await _unitOfWork.Warehouses.AddAsync(warehouse);

        await _unitOfWork.Users.AddAsync(new User
        {
            CreatedAt = DateTime.UtcNow,
            Email = createWarehouseDto.Email,
            PasswordHash = BcryptHelper.Hash(createWarehouseDto.Password),
            WarehouseId = warehouse.Id,
            CityId = createWarehouseDto.CityId,
            RoleId = role.Id,
            CompanyName = createWarehouseDto.Name,
            FirstName = createWarehouseDto.Name,
            CountryId = city.CountryId
        });

        await _unitOfWork.SaveChangesAsync();
        await _unitOfWork.CommitAsync();

        return ApiResponse<bool>.Success(warehouse.Id > 0, null, "Almacén creado exitosamente");
    }
}
