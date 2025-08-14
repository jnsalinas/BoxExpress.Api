using AutoMapper;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Application.Interfaces;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Enums;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Interfaces;

namespace BoxExpress.Application.Services;

public class ProductLoanService : IProductLoanService
{
    private readonly IProductLoanRepository _productLoanRepository;
    private readonly IProductLoanDetailRepository _productLoanDetailRepository;
    private readonly IWarehouseInventoryRepository _warehouseInventoryRepository;
    private readonly IInventoryMovementRepository _inventoryMovementRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly IUserContext _userContext;
    private readonly IInventoryMovementService _inventoryMovementService;
    private readonly IInventoryHoldService _inventoryHoldService;
    private readonly IInventoryHoldRepository _inventoryHoldRepository;

    public ProductLoanService(
        IProductLoanRepository productLoanRepository,
        IProductLoanDetailRepository productLoanDetailRepository,
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext,
        IInventoryMovementService inventoryMovementService,
        IInventoryHoldService inventoryHoldService,
        IInventoryHoldRepository inventoryHoldRepository)
    {
        _productLoanRepository = productLoanRepository;
        _productLoanDetailRepository = productLoanDetailRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _inventoryMovementService = inventoryMovementService;
        _inventoryHoldService = inventoryHoldService;
        _inventoryHoldRepository = inventoryHoldRepository;
    }

    public async Task<ApiResponse<ProductLoanDto>> CreateAsync(CreateProductLoanDto dto)
    {
        try
        {
            var warehouseInventories = await _warehouseInventoryRepository.GetByWarehouseAndProductVariants(
                dto.WarehouseId, dto.Details.Select(x => x.ProductVariantId).ToList());

            // Validar que la bodega existe y tiene inventario disponible
            foreach (var detail in dto.Details)
            {
                var inventory = warehouseInventories.FirstOrDefault(x => x.ProductVariantId == detail.ProductVariantId);
                if (inventory == null || inventory.AvailableQuantity < detail.RequestedQuantity)
                {
                    return ApiResponse<ProductLoanDto>.Fail($"No hay suficiente inventario disponible para la variante {detail.ProductVariantId}");
                }
            }

            await _unitOfWork.BeginTransactionAsync();

            var productLoan = new ProductLoan
            {
                LoanDate = dto.LoanDate,
                ResponsibleName = dto.ResponsibleName,
                Notes = dto.Notes,
                WarehouseId = dto.WarehouseId,
                CreatedById = _userContext.UserId,
                Status = ProductLoanStatus.Pending
            };

            await _unitOfWork.ProductLoans.AddAsync(productLoan);

            // Crear los detalles
            foreach (var detailDto in dto.Details)
            {
                var detail = new ProductLoanDetail
                {
                    ProductLoanId = productLoan.Id,
                    ProductVariantId = detailDto.ProductVariantId,
                    RequestedQuantity = detailDto.RequestedQuantity,
                    DeliveredQuantity = 0,
                    ReturnedQuantity = 0,
                };
                await _unitOfWork.ProductLoanDetails.AddAsync(detail);
                await _inventoryHoldService.CreateInventoryHoldAsync(
                    warehouseInventories.First(x => x.ProductVariantId == detailDto.ProductVariantId),
                    detailDto.RequestedQuantity,
                    InventoryHoldType.ProductLoan,
                    InventoryHoldStatus.Active,
                    null,
                    null,
                    detail.Id
                );
            }

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return ApiResponse<ProductLoanDto>.Success(_mapper.Map<ProductLoanDto>(productLoan));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<ProductLoanDto>.Fail($"Error al crear el préstamo: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductLoanDto>> GetByIdAsync(int id)
    {
        try
        {
            var productLoan = await _productLoanRepository.GetByIdWithDetailsAsync(id);
            if (productLoan == null)
                return ApiResponse<ProductLoanDto>.Fail("Préstamo no encontrado");

            var dto = _mapper.Map<ProductLoanDto>(productLoan);

            // Calcular totales
            dto.TotalRequestedQuantity = productLoan.Details.Sum(d => d.RequestedQuantity);
            dto.TotalDeliveredQuantity = productLoan.Details.Sum(d => d.DeliveredQuantity);
            dto.TotalReturnedQuantity = productLoan.Details.Sum(d => d.ReturnedQuantity);
            dto.TotalPendingReturnQuantity = productLoan.Details.Sum(d => d.PendingReturnQuantity);

            return ApiResponse<ProductLoanDto>.Success(dto);
        }
        catch (Exception ex)
        {
            return ApiResponse<ProductLoanDto>.Fail($"Error al obtener el préstamo: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<ProductLoanDto>>> GetFilteredAsync(ProductLoanFilterDto filterDto)
    {
        try
        {
            var filter = _mapper.Map<ProductLoanFilter>(filterDto);
            var productLoans = await _productLoanRepository.GetFilteredAsync(filter);

            var dtos = _mapper.Map<IEnumerable<ProductLoanDto>>(productLoans);

            // Calcular totales para cada préstamo
            foreach (var dto in dtos)
            {
                var productLoan = productLoans.First(x => x.Id == dto.Id);
                dto.TotalRequestedQuantity = productLoan.Details.Sum(d => d.RequestedQuantity);
                dto.TotalDeliveredQuantity = productLoan.Details.Sum(d => d.DeliveredQuantity);
                dto.TotalReturnedQuantity = productLoan.Details.Sum(d => d.ReturnedQuantity);
                dto.TotalPendingReturnQuantity = productLoan.Details.Sum(d => d.PendingReturnQuantity);
            }

            return ApiResponse<IEnumerable<ProductLoanDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ProductLoanDto>>.Fail($"Error al obtener los préstamos: {ex.Message}");
        }
    }

    public async Task<ApiResponse<ProductLoanDto>> UpdateAsync(int id, UpdateProductLoanDto dto)
    {
        try
        {
            await _unitOfWork.BeginTransactionAsync();

            var productLoan = await _productLoanRepository.GetByIdAsync(id);
            if (productLoan == null)
                return ApiResponse<ProductLoanDto>.Fail("Préstamo no encontrado");

            // Solo permitir modificación si está en estado Pending o si es super admin
            if (productLoan.Status != ProductLoanStatus.Pending)
            {
                // Aquí deberías verificar si el usuario es super admin
                // Por ahora, solo permitimos modificación en estado Pending
                return ApiResponse<ProductLoanDto>.Fail("No se puede modificar un préstamo que ya está en proceso o completado");
            }

            productLoan.LoanDate = dto.LoanDate;
            productLoan.ResponsibleName = dto.ResponsibleName;
            productLoan.Notes = dto.Notes;
            productLoan.Status = dto.Status;
            productLoan.UpdatedAt = DateTime.UtcNow;

            if (dto.Status == ProductLoanStatus.CompletedOk || dto.Status == ProductLoanStatus.CompletedWithIssue)
            {
                productLoan.ProcessedAt = DateTime.UtcNow;
                productLoan.ProcessedById = _userContext.UserId;
            }

            //adjust inventory
            //await _inventoryMovementService.AdjustInventoryAsync(productLoan);
            await _unitOfWork.ProductLoans.UpdateAsync(productLoan);
            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitAsync();
            return ApiResponse<ProductLoanDto>.Success(_mapper.Map<ProductLoanDto>(productLoan));
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<ProductLoanDto>.Fail($"Error al actualizar el préstamo: {ex.Message}");
        }
    }

    public async Task<ApiResponse<IEnumerable<ProductLoanDetailDto>>> GetDetailsAsync(int productLoanId)
    {
        try
        {
            var details = await _productLoanDetailRepository.GetByProductLoanIdAsync(productLoanId);
            var dtos = _mapper.Map<IEnumerable<ProductLoanDetailDto>>(details);
            return ApiResponse<IEnumerable<ProductLoanDetailDto>>.Success(dtos);
        }
        catch (Exception ex)
        {
            return ApiResponse<IEnumerable<ProductLoanDetailDto>>.Fail($"Error al obtener los detalles: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> UpdateDetailsAsync(int productLoanId, CreateProductLoanDto dto)
    {
        try
        {
            var details = dto.Details;
            var productLoan = await _productLoanRepository.GetByIdAsync(productLoanId);
            if (productLoan == null)
                return ApiResponse<bool>.Fail("Préstamo no encontrado");

            if (productLoan.Status == ProductLoanStatus.CompletedOk || productLoan.Status == ProductLoanStatus.CompletedWithIssue)
                return ApiResponse<bool>.Fail("No se puede modificar un préstamo ya completado");

            await _unitOfWork.BeginTransactionAsync();

            var inventoryHolds = (await _inventoryHoldRepository.GetFilteredAsync(new InventoryHoldFilter()
            {
                ProductLoanId = productLoanId,
                Statuses = new List<InventoryHoldStatus>() { InventoryHoldStatus.Active, InventoryHoldStatus.PendingReturn },
                IsAll = true
            })).InventoryHolds;

            foreach (var detailDto in details)
            {
                var detail = await _productLoanDetailRepository.GetByIdAsync(detailDto.Id.Value);
                if (detail == null || detail.ProductLoanId != productLoanId)
                    continue;

                detail.DeliveredQuantity = detailDto.DeliveredQuantity;
                detail.ReturnedQuantity = detailDto.ReturnedQuantity;
                detail.UpdatedAt = DateTime.UtcNow;

                // Actualizar inventario
                var inventory = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(
                    productLoan.WarehouseId, detail.ProductVariantId);

                //falta el hold de la bodega
                //tener en cuenta hold 
                //si hay delivered quantity, hacer movimientod de inventario
                //si hay returned quantity, no mueve inventario pero se libera inventario
                var hold = inventoryHolds.FirstOrDefault(x => x.ProductLoanDetailId == detail.Id);
                if (hold != null)
                {
                    if (detail.ReturnedQuantity > 0 && detail.DeliveredQuantity > 0)
                        hold.Status = InventoryHoldStatus.PartialReturned;
                    else if (detail.ReturnedQuantity == detail.RequestedQuantity)
                        hold.Status = InventoryHoldStatus.Returned;
                    else if (detail.DeliveredQuantity == detail.RequestedQuantity)
                        hold.Status = InventoryHoldStatus.Released;
                    await _unitOfWork.InventoryHolds.UpdateAsync(hold);
                }

                if (inventory != null)
                {
                    // Solo crear movimiento si hay cambio en delivered quantity
                    if (detail.DeliveredQuantity != 0)
                    {
                        await _inventoryMovementService.AdjustInventoryAsync(new InventoryMovement()
                        {
                            ProductLoanDetailId = detail.Id,
                            CreatorId = _userContext.UserId,
                            WarehouseId = productLoan.WarehouseId,
                            ProductVariantId = detail.ProductVariantId,
                            Quantity = detail.DeliveredQuantity * -1,
                            MovementType = InventoryMovementType.LoanDelivered,
                            Reference = $"ProductLoan-{productLoanId}",
                            Notes = $"Préstamo: {productLoan.ResponsibleName} - Cambio: {detail.DeliveredQuantity}"
                        }, true, false);
                    }

                    if (detail.ReturnedQuantity != 0)
                    {
                        inventory.ReservedQuantity -= detail.ReturnedQuantity;
                        await _unitOfWork.Inventories.UpdateAsync(inventory);
                    }
                }

                await _unitOfWork.ProductLoanDetails.UpdateAsync(detail);
            }

            // Actualizar estado del préstamo
            var allDetails = await _unitOfWork.ProductLoanDetails.GetByProductLoanIdAsync(productLoanId);
            var totalRequested = allDetails.Sum(d => d.RequestedQuantity);
            var totalDelivered = allDetails.Sum(d => d.DeliveredQuantity);
            var totalReturned = allDetails.Sum(d => d.ReturnedQuantity);

            if (totalRequested == totalDelivered)
            {
                productLoan.Status = ProductLoanStatus.CompletedOk;
            }
            else if (totalReturned > 0)
            {
                productLoan.Status = ProductLoanStatus.CompletedOk;//CompletedWithIssue;
            }

            productLoan.Notes = dto.Notes;
            productLoan.ProcessedAt = DateTime.UtcNow;
            productLoan.ProcessedById = _userContext.UserId;
            await _unitOfWork.ProductLoans.UpdateAsync(productLoan);

            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            await _unitOfWork.RollbackAsync();
            return ApiResponse<bool>.Fail($"Error al actualizar los detalles: {ex.Message}");
        }

        await _unitOfWork.CommitAsync();
        return ApiResponse<bool>.Success(true);
    }
}