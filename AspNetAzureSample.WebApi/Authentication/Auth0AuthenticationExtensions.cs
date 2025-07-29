using AspNetAzureSample.Authorization;
using AspNetAzureSample.Configuration;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text.Json;

namespace AspNetAzureSample.Authentication
{
    public static class Auth0AuthenticationExtensions
    {
        public static void ConfigureAuthentication(this AuthenticationBuilder authenticationBuilder,
                                                   ConfigurationManager configuration,
                                                   ILogger logger)
        {
            var auth0Options = new Auth0Options();
            configuration.Bind(Auth0Options.Name, auth0Options);

            if (!auth0Options.Enable)
                return;

            authenticationBuilder.AddJwtBearer(MultiSchemeAuthenticationExtensions.Auth0Scheme, options =>
            {
                options.Authority = auth0Options.Authority;
                options.Audience = auth0Options.Audience;
                options.Events = new CustomJwtBearerEvents(logger);
                options.TokenValidationParameters.NameClaimType = auth0Options.NameClaimType;

                var requiredOrgId = auth0Options.OrganizationId;
                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        return ValidateToken(context, requiredOrgId);
                    },

                    OnChallenge = CustomizeResponse
                };
            });
        }

        public static Task ValidateToken(TokenValidatedContext context, string organizationId)
        {
            var organizationIdClaim = context.Principal?.FindFirst(Claims.OrganizationIdClaimType);

            if (string.IsNullOrEmpty(organizationId) || organizationIdClaim == null)
            {
                context.Fail(MissingOrganizationMessage);
                return Task.CompletedTask;
            }

            if (organizationIdClaim.Value != organizationId)
                context.Fail($"Token does not contain the required organization ID claim or its value is incorrect. Required: '{organizationId}'");

            return Task.CompletedTask;
        }

        public static Task CustomizeResponse(JwtBearerChallengeContext context)
        {
            if (context.AuthenticateFailure == null)
                return Task.CompletedTask;

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";
            var errorResponse = new
            {
                status = StatusCodes.Status401Unauthorized,
                message = context.AuthenticateFailure.Message
            };
            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
        }

        internal const string MissingOrganizationMessage = "Required organization ID is not set in configuration.";
    }
}
