using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace BoxExpress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class CurrenciesController : ControllerBase
    {
        private readonly ICurrencyService _CurrencyService;
        public CurrenciesController(ICurrencyService CurrencyService)
        {
            _CurrencyService = CurrencyService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search()
        {
            var result = await _CurrencyService.GetAllAsync();
            return Ok(result);
        }
    }
}
