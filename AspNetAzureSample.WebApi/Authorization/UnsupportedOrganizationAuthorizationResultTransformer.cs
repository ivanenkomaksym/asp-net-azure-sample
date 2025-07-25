using AspNetAzureSample.Configuration;
using AspNetAzureSample.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Infrastructure;
using Microsoft.AspNetCore.Authorization.Policy;
using System.Text.Json;

namespace AspNetAzureSample.Authorization
{
    public class UnsupportedOrganizationAuthorizationResultTransformer : IAuthorizationMiddlewareResultHandler
    {
        private readonly IConfiguration Configuration;
        private readonly ILogger<UnsupportedOrganizationAuthorizationResultTransformer> Logger;

        public UnsupportedOrganizationAuthorizationResultTransformer(IConfiguration configuration,
                                                                     ILogger<UnsupportedOrganizationAuthorizationResultTransformer> logger)
        {
            Configuration = configuration;
            Logger = logger;
        }

        public async Task HandleAsync(RequestDelegate next,
                                      HttpContext context,
                                      AuthorizationPolicy policy,
                                      PolicyAuthorizationResult authorizeResult)
        {
            if (!authorizeResult.Succeeded)
            {
                var auth0Options = new Auth0Options();
                Configuration.Bind(Auth0Options.Name, auth0Options);

                var claimsRequirementFailed = authorizeResult.AuthorizationFailure?.FailedRequirements
                                             .OfType<ClaimsAuthorizationRequirement>()
                                             .Any(r => r.ClaimType == Claims.OrganizationIdClaimType)
                                             ?? false;

                if (claimsRequirementFailed)
                {
                    context.Response.StatusCode = StatusCodes.Status403Forbidden;
                    context.Response.ContentType = "application/json";

                    var responsePayload = new
                    {
                        error = "AuthorizationFailed",
                        message = $"Access denied. You are not part of the organization",
                        details = authorizeResult.AuthorizationFailure?.FailedRequirements
                                    .Select(r => $"Requirement Failed: {r.GetType().Name}")
                                    .ToList()
                    };

                    Logger.LogWarning($"Authorization failed for policy '{AuthorizationPolicies.OrganizationAccessPolicy}'. User is not part of the '{auth0Options.OrganizationId}'. Returning custom 403 response.");

                    await context.Response.WriteAsync(JsonSerializer.Serialize(responsePayload));
                    return;
                }
            }
        }
    }
}
