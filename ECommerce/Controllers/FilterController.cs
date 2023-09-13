using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ECommerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FilterController : ControllerBase
    {
        [HttpGet("mobiles")]
        public async Task<IActionResult> MobilesFilter(string color, string ram, string storage, string battery, 
            string screenSize, string resolution, string primaryCamera, string secondaryCamera)
        {
            return Ok();
        }
    }
}
