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

    public ProductLoanService(
        IProductLoanRepository productLoanRepository,
        IProductLoanDetailRepository productLoanDetailRepository,
        IWarehouseInventoryRepository warehouseInventoryRepository,
        IInventoryMovementRepository inventoryMovementRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        IUserContext userContext,
        IInventoryMovementService inventoryMovementService)
    {
        _productLoanRepository = productLoanRepository;
        _productLoanDetailRepository = productLoanDetailRepository;
        _warehouseInventoryRepository = warehouseInventoryRepository;
        _inventoryMovementRepository = inventoryMovementRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _userContext = userContext;
        _inventoryMovementService = inventoryMovementService;
    }

    public async Task<ApiResponse<ProductLoanDto>> CreateAsync(CreateProductLoanDto dto)
    {
        try
        {
            // Validar que la bodega existe y tiene inventario disponible
            foreach (var detail in dto.Details)
            {
                var inventory = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(
                    dto.WarehouseId, detail.ProductVariantId);

                if (inventory == null || inventory.AvailableQuantity < detail.RequestedQuantity)
                {
                    return ApiResponse<ProductLoanDto>.Fail($"No hay suficiente inventario disponible para la variante {detail.ProductVariantId}");
                }
            }

            var productLoan = new ProductLoan
            {
                LoanDate = dto.LoanDate,
                ResponsibleName = dto.ResponsibleName,
                Notes = dto.Notes,
                WarehouseId = dto.WarehouseId,
                CreatedById = _userContext.UserId,
                Status = ProductLoanStatus.Pending
            };

            // Crear los detalles
            foreach (var detailDto in dto.Details)
            {
                var detail = new ProductLoanDetail
                {
                    ProductVariantId = detailDto.ProductVariantId,
                    RequestedQuantity = detailDto.RequestedQuantity,
                    DeliveredQuantity = 0,
                    ReturnedQuantity = 0,
                    Notes = detailDto.Notes
                };
                productLoan.Details.Add(detail);
            }

            await _productLoanRepository.AddAsync(productLoan);
            return ApiResponse<ProductLoanDto>.Success(_mapper.Map<ProductLoanDto>(productLoan));
        }
        catch (Exception ex)
        {
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

    public async Task<ApiResponse<bool>> UpdateDetailsAsync(int productLoanId, List<UpdateProductLoanDetailDto> details)
    {
        try
        {
            var productLoan = await _productLoanRepository.GetByIdAsync(productLoanId);
            if (productLoan == null)
                return ApiResponse<bool>.Fail("Préstamo no encontrado");

            if (productLoan.Status == ProductLoanStatus.CompletedOk || productLoan.Status == ProductLoanStatus.CompletedWithIssue)
                return ApiResponse<bool>.Fail("No se puede modificar un préstamo ya completado");

            foreach (var detailDto in details)
            {
                var detail = await _productLoanDetailRepository.GetByIdAsync(detailDto.Id);
                if (detail == null || detail.ProductLoanId != productLoanId)
                    continue;

                var previousDelivered = detail.DeliveredQuantity;
                var previousReturned = detail.ReturnedQuantity;

                detail.DeliveredQuantity = detailDto.DeliveredQuantity;
                detail.ReturnedQuantity = detailDto.ReturnedQuantity;
                detail.Notes = detailDto.Notes;
                detail.UpdatedAt = DateTime.UtcNow;

                // Actualizar inventario
                var inventory = await _warehouseInventoryRepository.GetByWarehouseAndProductVariant(
                    productLoan.WarehouseId, detail.ProductVariantId);

                if (inventory != null)
                {
                    // Registrar movimiento de inventario
                    var movement = new InventoryMovement
                    {
                        WarehouseId = productLoan.WarehouseId,
                        ProductVariantId = detail.ProductVariantId,
                        Quantity = detailDto.DeliveredQuantity - previousDelivered,
                        MovementType = InventoryMovementType.Loan,
                        Reference = $"ProductLoan-{productLoanId}",
                        Notes = $"Préstamo: {productLoan.ResponsibleName} - {detailDto.Notes}"
                    };

                    await _inventoryMovementRepository.AddAsync(movement);

                    // Actualizar inventario
                    inventory.ReservedQuantity += (detailDto.DeliveredQuantity - previousDelivered);
                    inventory.PendingReturnQuantity += (detailDto.ReturnedQuantity - previousReturned);
                    await _warehouseInventoryRepository.UpdateAsync(inventory);
                }

                await _productLoanDetailRepository.UpdateAsync(detail);
            }

            // Actualizar estado del préstamo
            var allDetails = await _productLoanDetailRepository.GetByProductLoanIdAsync(productLoanId);
            var totalDelivered = allDetails.Sum(d => d.DeliveredQuantity);
            var totalReturned = allDetails.Sum(d => d.ReturnedQuantity);

            if (totalDelivered > 0)
            {
                // Determinar el estado basado en las cantidades
                if (totalReturned >= totalDelivered)
                {
                    // Si se devolvió todo o más, está completado OK
                    productLoan.Status = ProductLoanStatus.CompletedOk;
                }
                else if (totalReturned > 0)
                {
                    // Si se devolvió algo pero no todo, está completado con novedad
                    productLoan.Status = ProductLoanStatus.CompletedWithIssue;
                }
                else
                {
                    // Si no se ha devuelto nada, está en proceso
                    productLoan.Status = ProductLoanStatus.InProcess;
                }

                if (productLoan.Status == ProductLoanStatus.CompletedOk || productLoan.Status == ProductLoanStatus.CompletedWithIssue)
                {
                    productLoan.ProcessedAt = DateTime.UtcNow;
                    productLoan.ProcessedById = _userContext.UserId;
                }
                await _productLoanRepository.UpdateAsync(productLoan);
            }

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.Success(true);
        }
        catch (Exception ex)
        {
            return ApiResponse<bool>.Fail($"Error al actualizar los detalles: {ex.Message}");
        }
    }
}