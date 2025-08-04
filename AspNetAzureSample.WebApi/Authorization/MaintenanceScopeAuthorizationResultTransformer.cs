using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Text.Json;

namespace AspNetAzureSample.Authorization
{
    public class MaintenanceScopeAuthorizationResultTransformer : IAuthorizationMiddlewareResultHandler
    {
        public async Task HandleAsync(RequestDelegate next,
                                      HttpContext context,
                                      AuthorizationPolicy policy,
                                      PolicyAuthorizationResult authorizeResult)
        {
            // If the authorization failed
            if (authorizeResult.Forbidden && authorizeResult.AuthorizationFailure != null)
            {
                var failedScopeRequirement = authorizeResult.AuthorizationFailure.FailedRequirements.OfType<ClaimsAuthorizationRequirement>()
                                             .Where(r => r.ClaimType == "scope").FirstOrDefault();

                if (failedScopeRequirement != null)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";
                    // Handle authorization failure with custom message
                    var responsePayload = new
                    {
                        error = "AuthorizationFailed",
                        message = $"Forbidden. Current user does not have a specified scope to access this resource.",
                        details = $"Requirement Failed: {failedScopeRequirement.GetType().Name}"
                    };
                    
                    await context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload));
                    return;
                }
            }
        }
    }
}
