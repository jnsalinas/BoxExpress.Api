using BoxExpress.Application.Interfaces;
using BoxExpress.Application.Dtos.Integrations.Routing;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Constants;
using BoxExpress.Domain.Interfaces;
using System.Globalization;
using BoxExpress.Application.Dtos;
using Newtonsoft.Json;

namespace BoxExpress.Application.Services;

public class RoutePlanningService : IRoutePlanningService
{
    private readonly IRoutePlanningClient _routePlanningClient;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IOrderService _orderService;
    public RoutePlanningService(IRoutePlanningClient routePlanningClient, IOrderRepository orderRepository, IOrderStatusRepository orderStatusRepository, IOrderService orderService)
    {
        _routePlanningClient = routePlanningClient;
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;
        _orderService = orderService;
    }

    public async Task<ApiResponse<RoutingResponseCreatePlanDto>> CreatePlanAsync()
    {
        var statusid = 2; //_orderStatusRepository.GetByNameAsync(OrderStatusConstants.Scheduled).Id;
        OrderFilter filter = new OrderFilter
        {
            IsAll = true,
            StartScheduledDate = DateTime.Now,
            EndScheduledDate = DateTime.Now,
            StatusId = statusid,
            TimeSlotId = 0,
        };

        var (orders, totalCount) = await _orderRepository.GetFilteredAsync(filter);

        if (orders.Count == 0)
        {
            return ApiResponse<RoutingResponseCreatePlanDto>.Fail("No hay órdenes programadas para hoy.");
        }

        // ApiResponse<List<OrderExcelUploadResponseDto>> results = await _orderService.BulkChangeStatusAsync(new BulkChangeOrdersStatusDto()
        // {
        //     CourierName = "Mensajeros propios",
        //     DeliveryProviderId = 2, //todo buscar por name no se porque falla :( 
        //     OrderIds = orders.Select(o => o.Id).ToList(),
        //     StatusId = 3//_orderStatusRepository.GetByNameAsync(OrderStatusConstants.OnTheWay).Id// 3 //todo hacerlo por el repo buscar por name no se porque falla :( _orderStatusRepository.GetByNameAsync(OrderStatusConstants.OnTheWay).Id
        // });

        // if (results.Data != null && results.Data.Any(x => x.IsLoaded))
        // {
        //     orders = orders.Where(o => results.Data.Any(x => x.Id == o.Id && x.IsLoaded)).ToList();
        // }
        // else
        // {
        //     return ApiResponse<RoutingResponseCreatePlanDto>.Fail(results.Message ?? "Error al cambiar el estado de las órdenes");
        // }

        var ordersGroupByWarehouse = orders.GroupBy(o => o.WarehouseId).ToList();
        List<RoutingStopDto> stops = new List<RoutingStopDto>();
        List<RoutingResponseCreatePlanDto> results = new List<RoutingResponseCreatePlanDto>();

        foreach (var group in ordersGroupByWarehouse)
        {
            stops = new List<RoutingStopDto>();
            var warehouse = group.Key;
            string planName = group.ToList().First().Warehouse?.Name.ToUpper() + "-" + DateTime.Now.ToShortDateString() + "- Desde BoxYa";

            foreach (var stop in group.ToList())
            {
                stops.Add(new RoutingStopDto()
                {
                    Label = stop.Id.ToString() + " - " + stop.Client.FirstName + " " + stop.Client.LastName + " - " + stop.Store.Name,
                    Location = new RoutingLocationDto
                    {
                        Label = stop.ClientAddress.Address + " " + stop.ClientAddress.Complement + " " + stop.ClientAddress.PostalCode,
                        Lat = stop.ClientAddress.Latitude,
                        Lng = stop.ClientAddress.Longitude,
                        City = stop.ClientAddress.City?.Name,
                        Country = stop.ClientAddress.City?.Country?.Name,
                        PostalCode = !string.IsNullOrEmpty(stop.ClientAddress.PostalCode) ? stop.ClientAddress.PostalCode : null,
                    },
                    Comments = stop.Notes + "\n" + GetProductsComments(stop.OrderItems.ToList()),
                    ExternalId = stop.ClientAddress.Address,
                    LocationDetails = stop.ClientAddress.Complement + "-" + stop.ClientAddress.PostalCode + "-" + stop.ClientAddress.Zip,
                    Email = !string.IsNullOrEmpty(stop.Client.Email) ? stop.Client.Email : null,
                    ReferencePerson = stop.Client.FirstName + " " + stop.Client.LastName,
                    Phone = stop.Client.Phone,
                    Price = stop.TotalAmount.ToString(),
                    CustomFields = new Dictionary<string, string> { { "valor", stop.TotalAmount.ToString() } },
                    Status = "pending",
                });
            }

            results.Add(await _routePlanningClient.CreatePlanAsync(new RoutingCreatePlanDto
            {
                Label = planName,
                Stops = stops,
            }));
        }

        return ApiResponse<RoutingResponseCreatePlanDto>.Success(new RoutingResponseCreatePlanDto()
        {
            OrderIds = orders.Select(o => o.Id).ToList(),
            PlanNames = results.SelectMany(r => r.PlanNames).ToList(),
        }, null, "Plan creado exitosamente");
    }

    private string GetProductsComments(List<OrderItem> orderItems)
    {
        string comments = string.Empty;
        foreach (var item in orderItems)
        {
            comments += $"{item.Quantity} - {item.ProductVariant.Product.Name}  {item.ProductVariant.Name} - {item.ProductVariant.Sku} \n";
        }
        return comments;
    }

    public async Task<ApiResponse<bool>> UpdateStatusAsync(RoutingUpdateStatusDto dto)
    {
        var statuses = await _orderStatusRepository.GetAllAsync();
        int statusId = dto.Data.Status == "completed" ? statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.Delivered)?.Id ?? 0 : dto.Data.Status == "canceled" ? statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.Cancelled)?.Id ?? 0 : 0;

        var resultUpdateStatus = await _orderService.UpdateStatusAsync(Convert.ToInt32(dto.Data.Label.Split("-")[0]), statusId, new ChangeStatusDto() { Comments = "Desde integración SmartMoneky: " + dto.Data.Reports.FirstOrDefault()?.Comments ?? string.Empty });
        return ApiResponse<bool>.Success(resultUpdateStatus.Data?.Id != null);
    }
}