using AspNetAzureSample.Authorization;
using AspNetAzureSample.Configuration;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
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
                options.Events = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                {
                    OnTokenValidated = context =>
                    {
                        // Find the organization ID claim from the authenticated principal
                        var organizationIdClaim = context.Principal?.FindFirst(Claims.OrganizationIdClaimType);

                        // If the claim is missing OR its value does not match the required ID,
                        // then reject the token by failing the authentication context.
                        if (string.IsNullOrEmpty(requiredOrgId) || organizationIdClaim == null || organizationIdClaim.Value != requiredOrgId)
                        {
                            // This will prevent the creation of an authenticated principal for this scheme
                            context.Fail($"Token does not contain the required organization ID claim or its value is incorrect. Required: '{requiredOrgId}'");
                        }

                        return Task.CompletedTask;
                    },

                    OnChallenge = context =>
                    {
                        // Customize challenge response for Auth0 if token is invalid or missing
                        if (context.AuthenticateFailure != null)
                        {
                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            context.Response.ContentType = "application/json";
                            var errorResponse = new
                            {
                                status = StatusCodes.Status401Unauthorized,
                                message = context.AuthenticateFailure.Message // Use the message from context.Fail() above
                            };
                            return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                        }
                        // For other challenges, like no token provided, let default handle.
                        return Task.CompletedTask;
                    }
                };
            });
        }
    }
}
