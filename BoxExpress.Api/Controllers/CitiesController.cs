using BoxExpress.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using BoxExpress.Application.Dtos;

namespace BoxExpress.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CitiesController : ControllerBase
    {
        private readonly ICityService _cityService;
        public CitiesController(ICityService cityService)
        {
            _cityService = cityService;
        }

        [HttpPost("search")]
        public async Task<IActionResult> Search(CityFilterDto filter)
        {
            var result = await _cityService.GetAllAsync(filter);
            return Ok(result);
        }
    }
}
