using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Authorization;

namespace AspNetAzureSample.Authorization
{
    public class MinimumAgeAuthorizationResultTransformer : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(RequestDelegate next,
                                      HttpContext context,
                                      AuthorizationPolicy policy,
                                      PolicyAuthorizationResult authorizeResult)
        {
            // If the authorization failed
            if (!authorizeResult.Succeeded)
            {
                if (authorizeResult.AuthorizationFailure?.FailureReasons.FirstOrDefault() is MinimumAgeFailureReason minimumAgeFailureReason)
                {
                    // Handle authorization failure with custom message
                    var failureReason = minimumAgeFailureReason.Message;
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    await context.Response.WriteAsync($"Authorization failed: {failureReason}");
                    return;
                }
            }
        }
    }
}
