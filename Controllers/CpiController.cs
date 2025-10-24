using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CpiService.Models;
using CpiService.Services;

namespace CpiService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class CpiController : ControllerBase
    {
        private readonly CpiCacheService _cpiService;

        public CpiController(CpiCacheService cpiService)
        {
            _cpiService = cpiService;
        }

        [HttpGet]
        public async Task<IActionResult> GetCpi([FromQuery] int year, [FromQuery] string month)
        {
            var data = await _cpiService.GetCpiAsync(year, month);
            if (data == null)
                return NotFound("No CPI data found for given month/year.");
            return Ok(data);
        }
    }
}