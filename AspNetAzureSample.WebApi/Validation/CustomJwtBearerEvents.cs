using AspNetAzureSample.Extensions;
using Microsoft.AspNetCore.Authentication.JwtBearer;

namespace AspNetAzureSample.Validation
{
    public class CustomJwtBearerEvents : JwtBearerEvents
    {
        private readonly ILogger Logger;

        public CustomJwtBearerEvents(ILogger logger)
        {
            Logger = logger;
        }

        public override Task AuthenticationFailed(AuthenticationFailedContext context)
        {
            Logger.AuthenticationFailed(context.Exception);
            return base.AuthenticationFailed(context);
        }

        /// <summary>
        /// This method contains the logic that validates the user's tenant and normalizes claims.
        /// </summary>
        /// <param name="context">The validated token context</param>
        /// <returns>A task</returns>
        public override Task TokenValidated(TokenValidatedContext context)
        {
            var principal = context.Principal;
            if (principal == null)
                return base.TokenValidated(context);

            var issuerValue = principal.GetIssuerValue();
            if (issuerValue == null)
                return base.TokenValidated(context);

            Logger.TokenValidationSucceeded(principal.GetObjectIdentifierValue(), issuerValue, principal.GetScopeValue(), principal.GetRoleValue(), principal.GetRolesValue());
            return base.TokenValidated(context);
        }
    }
}
