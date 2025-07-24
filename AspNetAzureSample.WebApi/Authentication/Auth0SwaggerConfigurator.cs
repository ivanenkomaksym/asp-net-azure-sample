using AspNetAzureSample.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AspNetAzureSample.Authentication
{
    public class Auth0SwaggerConfigurator(IOptions<Auth0Options> auth0Options) : ISwaggerConfigurator
    {
        public OpenApiOAuthFlow BuildImplicitFlow()
        {
            return new OpenApiOAuthFlow
            {
                AuthorizationUrl = new Uri($"{Auth0Options.Authority}authorize"),
                TokenUrl = new Uri($"{Auth0Options.Authority}auth2/token")
            };
        }

        public void ConfigureSwaggerUIOptions(SwaggerUIOptions swaggerUIOptions)
        {
            var additionalQueryStringParams = new Dictionary<string, string> {
                { "audience", Auth0Options.Audience }
            };

            if (!string.IsNullOrWhiteSpace(Auth0Options.OrganizationId))
                additionalQueryStringParams.Add("organization", Auth0Options.OrganizationId);

            swaggerUIOptions.OAuthConfigObject.AdditionalQueryStringParams = additionalQueryStringParams;
        }

        private readonly Auth0Options Auth0Options = auth0Options.Value ?? throw new ArgumentNullException(nameof(auth0Options));
    }
}
