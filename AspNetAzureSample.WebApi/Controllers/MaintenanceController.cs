using AspNetAzureSample.Security;
using AspNetAzureSample.UserProviders;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspNetAzureSample.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MaintenanceController : ControllerBase
    {
        public MaintenanceController(IUserProvider userProvider)
        {
            UserProvider = userProvider;
        }

        [Authorize(Policy = AuthorizationPolicies.ApplicationAccessPolicy)]
        [HttpPost]
        public IActionResult Reinitialize()
        {
            var _ = UserProvider.GetUserName(HttpContext);

            return Ok();
        }

        [Authorize(Policy = AuthorizationPolicies.CycleManagementPolicy)]
        [HttpPost("InitializeCycle")]
        public IActionResult InitializeCycle()
        {
            var _ = UserProvider.GetUserName(HttpContext);

            return Ok();
        }

        private readonly IUserProvider UserProvider;
    }
}
