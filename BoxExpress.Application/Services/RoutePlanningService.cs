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
using BoxExpress.Domain.Enums;

namespace BoxExpress.Application.Services;

public class RoutePlanningService : IRoutePlanningService
{
    private readonly IRoutePlanningClient _routePlanningClient;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderStatusRepository _orderStatusRepository;
    private readonly IOrderService _orderService;
    private readonly IUserContext _userContext;
    public RoutePlanningService(IRoutePlanningClient routePlanningClient, IOrderRepository orderRepository, IOrderStatusRepository orderStatusRepository, IOrderService orderService, IUserContext userContext)
    {
        _routePlanningClient = routePlanningClient;
        _orderRepository = orderRepository;
        _orderStatusRepository = orderStatusRepository;
        _orderService = orderService;
        _userContext = userContext;
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
            CountryId = _userContext.CountryId != null ? _userContext.CountryId : null
        };

        var (orders, totalCount) = await _orderRepository.GetFilteredAsync(filter);

        if (orders.Count == 0)
        {
            return ApiResponse<RoutingResponseCreatePlanDto>.Fail("No hay órdenes programadas para hoy.");
        }

        ApiResponse<List<OrderExcelUploadResponseDto>> results = await _orderService.BulkChangeStatusAsync(new BulkChangeOrdersStatusDto()
        {
            CourierName = "Mensajeros propios",
            DeliveryProviderId = 2, //todo buscar por name no se porque falla :( 
            OrderIds = orders.Select(o => o.Id).ToList(),
            StatusId = 3//_orderStatusRepository.GetByNameAsync(OrderStatusConstants.OnTheWay).Id// 3 //todo hacerlo por el repo buscar por name no se porque falla :( _orderStatusRepository.GetByNameAsync(OrderStatusConstants.OnTheWay).Id
        });

        if (results.Data != null && results.Data.Any(x => x.IsLoaded))
        {
            orders = orders.Where(o => results.Data.Any(x => x.Id == o.Id && x.IsLoaded)).ToList();
        }
        else
        {
            return ApiResponse<RoutingResponseCreatePlanDto>.Fail(results.Message ?? "Error al cambiar el estado de las órdenes");
        }

        var ordersGroupByWarehouse = orders.GroupBy(o => o.WarehouseId).ToList();
        List<RoutingStopDto> stops = new List<RoutingStopDto>();
        List<RoutingResponseCreatePlanDto> resultsRouting = new List<RoutingResponseCreatePlanDto>();

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

            resultsRouting.Add(await _routePlanningClient.CreatePlanAsync(new RoutingCreatePlanDto
            {
                Label = planName,
                Stops = stops,
            }));
        }

        return ApiResponse<RoutingResponseCreatePlanDto>.Success(new RoutingResponseCreatePlanDto()
        {
            OrderIds = orders.Select(o => o.Id).ToList(),
            PlanNames = resultsRouting.Where(r => !string.IsNullOrEmpty(r.Label)).Select(r => r.Label).ToList(),
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
        //hacer validacion de status entregado cancelado o en ruta par esea orden
        if (!((dto?.Data?.Label?.Contains("-") ?? false) && (dto?.Data?.Status?.ToLower() == "completed" || dto?.Data?.Status?.ToLower() == "canceled")))
        {
            return ApiResponse<bool>.Success(false, null, "Label no válido, debe contener el id de la orden");
        }

        var statuses = await _orderStatusRepository.GetAllAsync();
        int? statusId = dto.Data.Status == "completed" ? statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.Delivered)?.Id ?? 0 : dto.Data.Status == "canceled" ? statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.Cancelled)?.Id ?? 0 : 0;

        if (statusId == null || statusId == 0)
        {
            return ApiResponse<bool>.Fail("Estado no permitido");
        }

        int orderId = Convert.ToInt32(dto.Data.Label.Split("-")[0]);
        var order = await _orderRepository.GetByIdAsync(orderId);
        if (order == null)
        {
            return ApiResponse<bool>.Fail("Orden no encontrada");
        }

        if (order.OrderStatusId != statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.Delivered)?.Id && order.OrderStatusId != statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.Cancelled)?.Id && order.OrderStatusId != statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.OnTheWay)?.Id && order.OrderStatusId != statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.Scheduled)?.Id)
        {
            return ApiResponse<bool>.Success(true, null, "El estado de la orden no permite actualización desde la integración");
        }

        if (order.OrderStatusId == statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.Scheduled)?.Id)
        {
            var resultUpdateStatusUpdate = await _orderService.UpdateStatusAsync(orderId, statuses.FirstOrDefault(s => s.Name == OrderStatusConstants.OnTheWay)!.Id, new ChangeStatusDto() { Comments = "Desde integración SmartMoneky: Cambio automatico de programado a en ruta previo a cambio de estado de real de la integracion " + dto.Data.Status + " - " + dto.Data.Reports.FirstOrDefault()?.Comments ?? string.Empty });
            if (resultUpdateStatusUpdate.Data?.Id == null)
            {
                return ApiResponse<bool>.Fail("Error al actualizar el estado de la orden de programado a en ruta antes de la actualización final");
            }
        }

        var resultUpdateStatus = await _orderService.UpdateStatusAsync(orderId, statusId.Value, new ChangeStatusDto() { Comments = "Desde integración SmartMoneky: " + dto.Data.Reports.FirstOrDefault()?.Comments ?? string.Empty });
        return ApiResponse<bool>.Success(resultUpdateStatus.Data?.Id != null);
    }
}