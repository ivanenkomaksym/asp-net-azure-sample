using AspNetAzureSample.Authentication;
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

        public static void ConfigureSwagger(this WebApplication app, ConfigurationManager configuration, ISwaggerConfigurator swaggerConfigurator)
        {
            var swaggerOptions = new SwaggerOptions();
            configuration.Bind(SwaggerOptions.Name, swaggerOptions);

            app.UseSwagger();
            app.UseSwaggerUI(swaggerUIOptions =>
            {
                swaggerUIOptions.SwaggerEndpoint("/swagger/v1/swagger.json", "WeatherForecast v1");
                swaggerUIOptions.OAuthClientId(swaggerOptions.ClientID);
                swaggerUIOptions.OAuthAppName("Swagger UI - My API");
                swaggerUIOptions.OAuthScopeSeparator(" ");
                swaggerUIOptions.OAuth2RedirectUrl(swaggerOptions.RedirectUrl);

                swaggerConfigurator.ConfigureSwaggerUIOptions(swaggerUIOptions);
            });
        }
    }
}
