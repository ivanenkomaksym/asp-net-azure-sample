using Microsoft.AspNetCore.Authorization;

namespace AspNetAzureSample.Authorization
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var httpContext = (context.Resource as HttpContext);

            if (httpContext != null && httpContext.Request.Headers.TryGetValue(requirement.HeaderName, out var headerValue))
            {
                var result = ushort.TryParse(headerValue, out var age);
                if (!result)
                {
                    var missingAgeReason = new MinimumAgeFailureReason(this, "Incorrect age format.");
                    context.Fail(missingAgeReason);
                    return Task.CompletedTask;
                }

                if (age < requirement.MinimumAge)
                {
                    var missingAgeReason = new MinimumAgeFailureReason(this, "Underage.");
                    context.Fail(missingAgeReason);
                    return Task.CompletedTask;
                }
                 
                context.Succeed(requirement); // Authorization succeeds
                return Task.CompletedTask;
            }

            // Fail if header is missing or value is incorrect
            var underageReason = new MinimumAgeFailureReason(this, "Missing age.");
            context.Fail(underageReason);
            return Task.CompletedTask;
        }
    }
}
