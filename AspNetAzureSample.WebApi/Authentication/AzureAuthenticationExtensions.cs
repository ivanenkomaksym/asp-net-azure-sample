using AspNetAzureSample.Configuration;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Identity.Web;
using Microsoft.IdentityModel.Tokens;

namespace AspNetAzureSample.Authentication
{
    public static class AzureAuthenticationExtensions
    {
        public static void ConfigureAuthentication(this AuthenticationBuilder authenticationBuilder,
                                                   ConfigurationManager configuration,
                                                   ILogger logger)
        {
            var azureadOptions = new AzureADOptions();
            configuration.Bind(AzureADOptions.Name, azureadOptions);

            if (!azureadOptions.Enable)
                return;

            authenticationBuilder.AddMicrosoftIdentityWebApi(jwtOptions =>
            {
                configuration.GetSection(AzureADOptions.Name).Bind(jwtOptions);
                jwtOptions.TokenValidationParameters.IssuerValidator = (string issuer,
                                                                        SecurityToken securityToken,
                                                                        TokenValidationParameters validationParameters) =>
                {
                    return CustomIssuerValidator.ValidateSpecificIssuers(issuer, securityToken, validationParameters, azureadOptions.AcceptedTenantIds);
                };
                jwtOptions.Events = new CustomJwtBearerEvents(logger);
            },
            msIdentityOptions =>
            {
                configuration.GetSection(AzureADOptions.Name).Bind(msIdentityOptions);
            });
        }
    }
}
