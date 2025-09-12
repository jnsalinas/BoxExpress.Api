using BoxExpress.Application.Dtos;
using BoxExpress.Application.Integrations.Shopify;
using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.Data;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using BoxExpress.Api.Attributes;

namespace BoxExpress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]

    public class ShopifyController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public ShopifyController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("test")]
        public async Task<IActionResult> CreateTest()
        {
            var shopId = Request.Headers["X-Shopify-Shop-Domain"].FirstOrDefault();
            using var reader = new StreamReader(Request.Body);
            var jsonBody = await reader.ReadToEndAsync();
            jsonBody += "|headers: " + JsonConvert.SerializeObject(Request.Headers);

            var result = await _orderService.AddOrderMockAsync(jsonBody);
            return Ok(result);
        }

        [HttpPost()]
        [ServiceFilter(typeof(ShopifyTokenAttribute))]
        public async Task<IActionResult> Create([FromBody] ShopifyOrderDto orderDto)
        {
            var storeDomain = Request.Headers["X-Shopify-Shop-Domain"].FirstOrDefault();
            orderDto.Store_Domain = storeDomain;
            var result = await _orderService.AddOrderAsync(orderDto);
            return Ok(result);
        }

        [HttpPost("webhook/{storeId:int}")]
        [ServiceFilter(typeof(ShopifyTokenAttribute))]
        public async Task<IActionResult> CreateWebhook(
            [FromRoute] int storeId,
            [FromBody] ShopifyOrderDto orderDto
        )
        {
            orderDto.StoreId = storeId;
            var result = await _orderService.AddOrderAsync(orderDto);
            return Ok(result);
        }

        [HttpPost("webhooksspending/{publicId:guid}")]
        [ServiceFilter(typeof(ShopifyTokenAttribute))]
        public async Task<IActionResult> CreateWebhook(
            [FromRoute] Guid publicId,
            [FromBody] ShopifyOrderDto orderDto
        )
        {
            orderDto.PublicId = publicId;
            var result = await _orderService.AddOrderAsync(orderDto);
            return Ok(result);
        }
    }
}
