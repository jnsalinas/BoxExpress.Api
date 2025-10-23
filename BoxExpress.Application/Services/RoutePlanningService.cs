using BoxExpress.Application.Interfaces;
using BoxExpress.Application.Dtos.Integrations.Routing;
using BoxExpress.Application.Dtos.Common;
using BoxExpress.Domain.Entities;
using BoxExpress.Domain.Filters;
using BoxExpress.Domain.Constants;
using BoxExpress.Domain.Interfaces;
using System.Globalization;
using BoxExpress.Application.Dtos;

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
            TimeSlotId = 0
        };

        var (orders, totalCount) = await _orderRepository.GetFilteredAsync(filter);

        if (orders.Count == 0)
        {
            return ApiResponse<RoutingResponseCreatePlanDto>.Fail("No hay órdenes programadas para hoy.");
        }

        ApiResponse<List<OrderExcelUploadResponseDto>> results = await _orderService.BulkChangeStatusAsync(new BulkChangeOrdersStatusDto()
        {
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

        CultureInfo myCI = new CultureInfo("es-CO");
        string dayName = DateTime.Now.ToString("dddd", myCI);


        string planName = "Plan-" + DateTime.Now.ToShortDateString() + "-" + dayName;
        RoutingCreatePlanDto request = new RoutingCreatePlanDto
        {
            Label = planName,
            Stops = orders.Select(o => new RoutingStopDto
            {
                Label = o.Id.ToString() + "-" + o.Code + "-" + o.Client.FirstName + " " + o.Client.LastName,
                Location = new RoutingLocationDto
                {
                    Label = o.ClientAddress.Address,
                    Lat = o.ClientAddress.Latitude,
                    Lng = o.ClientAddress.Longitude,
                    City = o.ClientAddress.City?.Name,
                    Country = o.ClientAddress.City?.Country?.Name,
                    PostalCode = o.ClientAddress.PostalCode
                },
                Comments = GetProductsComments(o.OrderItems.ToList()),
                ExternalId = o.ClientAddress.Address,
                LocationDetails = o.ClientAddress.Complement + "-" + o.ClientAddress.PostalCode + "-" + o.ClientAddress.Zip,
                Email = o.Client.Email,
                ReferencePerson = o.Client.FirstName + " " + o.Client.LastName,
                Phone = o.Client.Phone,
                Price = o.TotalAmount.ToString(),
                CustomFields = new Dictionary<string, string> { { "valor", o.TotalAmount.ToString() } },
                Status = "pending",
            }).ToList()
        };

        var response = await _routePlanningClient.CreatePlanAsync(request);
        return ApiResponse<RoutingResponseCreatePlanDto>.Success(new RoutingResponseCreatePlanDto()
        {
            OrderIds = orders.Select(o => o.Id).ToList(),
            PlanName = planName
        }, null, "Plan creado exitosamente");
    }

    private string GetProductsComments(List<OrderItem> orderItems)
    {
        string comments = string.Empty;
        foreach (var item in orderItems)
        {
            comments += $"Producto: {item.ProductVariant.Product.Name} - SKU: {item.ProductVariant.Product.Sku} - Cantidad: {item.Quantity};";
        }
        return comments;
    }
}