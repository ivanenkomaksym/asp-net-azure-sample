using AspNetAzureSample.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetAzureSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MaintenanceController : ControllerBase
    {
        [Authorize(Policy = AuthorizationPolicies.ApplicationAccessPolicy)]
        [HttpPost]
        public IActionResult Reinitialize()
        {
            return Ok();
        }
    }
}
