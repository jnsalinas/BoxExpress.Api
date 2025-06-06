using BoxExpress.Application.Dtos;
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;

namespace BoxExpress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ShopifyController : ControllerBase
    {
        // private readonly shopifyService _ShopifyService;

        // public ShopifyController(IShopifyService ShopifyService)
        // {
        //     _ShopifyService = ShopifyService;
        // }

        [HttpPost()]
        public async Task<IActionResult> Create([FromBody] LoginDto loginDto)
        {
            // var result = await _ShopifyService.ShopifyenticateAsync(loginDto);
            return Ok();
        }
    }
}
