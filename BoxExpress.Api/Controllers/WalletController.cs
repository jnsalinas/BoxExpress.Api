// BoxExpress.Api/Controllers/OrdersController.cs
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;
using System.Linq;
using System.Security.Claims;
using BoxExpress.Domain.Constants;
using BoxExpress.Application.Dtos.Common;
using Microsoft.AspNetCore.Authorization;

namespace BoxExpress.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WalletController : ControllerBase
{
    private readonly IStoreService _storeService;
    public WalletController(IStoreService storeService)
    {
        _storeService = storeService;
    }

    [HttpGet("summary")]
    public async Task<IActionResult> Summary()
    {
        ApiResponse<StoreDto?> result = new ApiResponse<StoreDto?>();
        var role = User.FindFirst(ClaimTypes.Role)?.Value;
        if (role?.ToLower() == RolConstants.Store)
        {
            var storeIdvalue = User.FindFirst("StoreId")?.Value;
            if (storeIdvalue == null)
            {
                return BadRequest("StoreId is required for warehouse role.");
            }

            result = await _storeService.GetByIdAsync(int.Parse(storeIdvalue)); //todo pendiente hacer funcion para obtener el resumen de todos los wallets
        }
        else if (role?.ToLower() == RolConstants.Admin)
        {
            result = await _storeService.GetBalanceSummary();
        }
       

        return Ok(result);
    }
}
