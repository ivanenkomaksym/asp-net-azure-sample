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

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecast v1");
                c.OAuthClientId(swaggerOptions.ClientID);
                c.OAuthClientSecret(swaggerOptions.ClientSecret);
                c.OAuthRealm(azureadOptions.ClientID);
                c.OAuthScopeSeparator(" ");
                // Needed for AuthorizationUrl = new Uri($"{host}/{tenantId}/oauth2/authorize")
                if (azureadOptions.ClientID != null)
                    c.OAuthConfigObject.AdditionalQueryStringParams = new Dictionary<string, string> { { "resource", azureadOptions.ClientID } };
            });
        }
    }
}
