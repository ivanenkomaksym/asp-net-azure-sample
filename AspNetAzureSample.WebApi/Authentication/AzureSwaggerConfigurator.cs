using AspNetAzureSample.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AspNetAzureSample.Authentication
{
    public class AzureSwaggerConfigurator(IOptions<AzureADOptions> azureAdOptions) : ISwaggerConfigurator
    {
        public OpenApiOAuthFlow BuildImplicitFlow()
        {
            var host = AzureAdOptions.Instance;
            var tenantId = AzureAdOptions.TenantId;

            return new OpenApiOAuthFlow()
            {
                AuthorizationUrl = new Uri($"{host}/{tenantId}/oauth2/authorize"),
                TokenUrl = new Uri($"{host}/{tenantId}/oauth2/v2.0/token")
            };
        }

        public void ConfigureSwaggerUIOptions(SwaggerUIOptions swaggerUIOptions)
        {
            swaggerUIOptions.OAuthConfigObject.AdditionalQueryStringParams = new Dictionary<string, string> { { "resource", AzureAdOptions.ClientID } };
        }

        private readonly AzureADOptions AzureAdOptions = azureAdOptions.Value ?? throw new ArgumentNullException(nameof(azureAdOptions));
    }
}
