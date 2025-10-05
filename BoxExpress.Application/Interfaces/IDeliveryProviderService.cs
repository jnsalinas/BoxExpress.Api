using BoxExpress.Application.Dtos;
using BoxExpress.Application.Dtos.Common;

namespace BoxExpress.Application.Interfaces;

public interface IDeliveryProviderService
{
    Task<ApiResponse<IEnumerable<DeliveryProviderDto>>> GetAllAsync(DeliveryProviderFilterDto filter);
}
