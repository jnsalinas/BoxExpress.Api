using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IWarehouseService
{
    Task<ApiResponse<IEnumerable<WarehouseDto>>> GetAllAsync(WarehouseFilterDto filter);
    Task<ApiResponse<WarehouseDetailDto?>> GetByIdAsync(int id);
    Task<ApiResponse<bool>> AddInventoryToWarehouseAsync(int warehouseId, List<CreateProductWithVariantsDto> products);
    Task<ApiResponse<bool>> CreateTransferAsync(WarehouseInventoryTransferDto warehouseInventoryTransferDto);
    Task<ApiResponse<bool>> RejectTransferAsync(int transferId, int userId, string rejectionReason);
    Task<ApiResponse<bool>> AcceptTransferAsync(int transferId, int userId);
}
