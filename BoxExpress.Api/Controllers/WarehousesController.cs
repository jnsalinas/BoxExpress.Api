// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WarehousesController : ControllerBase
{
    private readonly IWarehouseService _warehouseService;
    private readonly IExcelExporter<WarehouseDto> _excelExporter;

    public WarehousesController(IWarehouseService warehouseService, IExcelExporter<WarehouseDto> excelExporter)
    {
        _excelExporter = excelExporter;
        _warehouseService = warehouseService;
    }

    [HttpPost("search")]
    public async Task<IActionResult> Search([FromBody] WarehouseFilterDto filter)
    {
        var result = await _warehouseService.GetAllAsync(filter);
        return Ok(result);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _warehouseService.GetByIdAsync(id);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    [HttpPost("{warehouseId}/inventory")]
    public async Task<IActionResult> AddInventoryToWarehouse(int warehouseId, [FromBody] List<CreateProductWithVariantsDto> products)
    {
        var result = await _warehouseService.AddInventoryToWarehouseAsync(warehouseId, products);
        if (result.Equals(null)) return NotFound();
        return Ok(result);
    }

    [HttpPost("{warehouseId}/transfers")]
    public async Task<IActionResult> CreateTransferAsync(int warehouseId, [FromBody] WarehouseInventoryTransferDto transferDto)
    {
        transferDto.FromWarehouseId = warehouseId;  // Asignar la bodega de origen
        var result = await _warehouseService.CreateTransferAsync(transferDto);
        return Ok(result);
    }

    // [HttpPost("transfers/{id}/accept")]
    // public async Task<IActionResult> AcceptTransferAsync(int id)
    // {
    //     var result = await _warehouseService.AcceptTransferAsync(id, 2); //todo: pasar el usuario de la sesion
    //     return Ok(result);
    // }

    // [HttpPost("transfers/{id}/reject")]
    // public async Task<IActionResult> RejectTransferAsync(int id, [FromBody] WarehouseInventoryTransferRejectDto warehouseInventoryTransferRejectDto)
    // {
    //     var result = await _warehouseService.RejectTransferAsync(id, 2, warehouseInventoryTransferRejectDto.Reason); //todo: pasar el usuario de la sesion
    //     return Ok(result);
    // }
}


// [Route("api/transfers")]
// [ApiController]
// public class TransferController : ControllerBase
// {
//     private readonly IWarehouseInventoryTransferRepository _transferRepository;
//     private readonly IUnitOfWork _unitOfWork;
//     private readonly IMapper _mapper;

//     public TransferController(
//         IWarehouseInventoryTransferRepository transferRepository,
//         IUnitOfWork unitOfWork,
//         IMapper mapper)
//     {
//         _transferRepository = transferRepository;
//         _unitOfWork = unitOfWork;
//         _mapper = mapper;
//     }

//     [HttpPost("{transferId}/accept")]
//     public async Task<ActionResult<ApiResponse<bool>>> AcceptTransferAsync(int transferId, [FromBody] TransferActionDto actionDto)
//     {
//         // Lógica para aceptar la transferencia
//     }

//     [HttpPost("{transferId}/reject")]
//     public async Task<ActionResult<ApiResponse<bool>>> RejectTransferAsync(int transferId, [FromBody] TransferActionDto actionDto)
//     {
//         // Lógica para rechazar la transferencia
//     }
// }
