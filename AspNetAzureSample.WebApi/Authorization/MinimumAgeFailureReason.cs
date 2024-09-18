using Microsoft.AspNetCore.Authorization;

namespace AspNetAzureSample.Authorization
{
    public class MinimumAgeFailureReason : AuthorizationFailureReason
    {
        public MinimumAgeFailureReason(IAuthorizationHandler handler, string message) : base(handler, message)
        {
        }
    }
}
