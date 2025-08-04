using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IProductLoanService
{
    Task<ApiResponse<ProductLoanDto>> CreateAsync(CreateProductLoanDto dto);
    Task<ApiResponse<ProductLoanDto>> GetByIdAsync(int id);
    Task<ApiResponse<IEnumerable<ProductLoanDto>>> GetFilteredAsync(ProductLoanFilterDto filter);
    Task<ApiResponse<ProductLoanDto>> UpdateAsync(int id, UpdateProductLoanDto dto);
    Task<ApiResponse<IEnumerable<ProductLoanDetailDto>>> GetDetailsAsync(int productLoanId);
    Task<ApiResponse<bool>> UpdateDetailsAsync(int productLoanId, List<UpdateProductLoanDetailDto> details);
} 