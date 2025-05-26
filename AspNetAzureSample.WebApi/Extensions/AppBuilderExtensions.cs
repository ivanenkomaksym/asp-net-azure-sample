using AspNetAzureSample.Configuration;

namespace AspNetAzureSample.Extensions
{
    public static class AppBuilderExtensions
    {
        public static void MigrateDatabase(this WebApplication app, ConfigurationManager configuration)
        {
            var storageOptions = new StorageOptions();
            configuration.Bind(StorageOptions.Name, storageOptions);

            if (storageOptions.StorageType != StorageOptions.StorageTypes.InMemory)
                app.MigrateDatabase();
        }

        public static void ConfigureSwagger(this WebApplication app, ConfigurationManager configuration)
        {
            var swaggerOptions = new SwaggerOptions();
            configuration.Bind(SwaggerOptions.Name, swaggerOptions);

            var azureadOptions = new AzureADOptions();
            configuration.Bind(AzureADOptions.Name, azureadOptions);

            var auth0Options = new Auth0Options();
            configuration.Bind(Auth0Options.Name, auth0Options);

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecast v1");
                c.OAuthClientId(swaggerOptions.ClientID);
                c.OAuthAppName("Swagger UI - My API");
                c.OAuthScopeSeparator(" ");
                c.OAuth2RedirectUrl(swaggerOptions.RedirectUrl);
                // Refactor this
                if (auth0Options.Enable && auth0Options.Audience != null)
                    c.OAuthConfigObject.AdditionalQueryStringParams = new Dictionary<string, string> { { "audience", auth0Options.Audience } };
                else if (azureadOptions.Enable)
                {
                    c.OAuthRealm(azureadOptions.ClientID);
                    if (azureadOptions.ClientID != null)
                        c.OAuthConfigObject.AdditionalQueryStringParams = new Dictionary<string, string> { { "resource", azureadOptions.ClientID } };
                }
            });
        }
    }
}
