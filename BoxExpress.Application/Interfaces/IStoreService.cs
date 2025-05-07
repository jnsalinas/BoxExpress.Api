using BoxExpress.Domain.Entities;
using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IStoreService
{
    Task<ApiResponse<StoreDto?>> GetByIdAsync(int storeId);
    Task<ApiResponse<IEnumerable<StoreDto>>> GetAllAsync(StoreFilterDto filter);
    Task<ApiResponse<bool>> AddStoreAsync(CreateStoreDto createStoreDto);
}
