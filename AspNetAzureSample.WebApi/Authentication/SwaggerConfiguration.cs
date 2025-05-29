using AspNetAzureSample.Configuration;
using Microsoft.Extensions.Options;

namespace AspNetAzureSample.Authentication
{
    public static class SwaggerConfiguration
    {   
        public static ISwaggerConfigurator BuildSwaggerConfigurator(ConfigurationManager configuration)
        {
            var azureAdOptions = new AzureADOptions();
            configuration.Bind(AzureADOptions.Name, azureAdOptions);

            var auth0Options = new Auth0Options();
            configuration.Bind(Auth0Options.Name, auth0Options);

            var swaggerOptions = new SwaggerOptions();
            configuration.Bind(SwaggerOptions.Name, swaggerOptions);

            return swaggerOptions.SecuritySchemeType switch
            {
                SwaggerSecuritySchemeType.Azure => new AzureSwaggerConfigurator(Options.Create(azureAdOptions)),
                SwaggerSecuritySchemeType.Auth0 => new Auth0SwaggerConfigurator(Options.Create(auth0Options)),
                _ => throw new InvalidOperationException("Unsupported security scheme type."),
            };
        }
    }
}
