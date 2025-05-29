using AspNetAzureSample.Configuration;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Authentication;

namespace AspNetAzureSample.Authentication
{
    public static class GoogleAuthenticationExtensions
    {
        public static void ConfigureAuthentication(this AuthenticationBuilder authenticationBuilder,
                                                   ConfigurationManager configuration,
                                                   ILogger logger)
        {
            var googleOptions = new GoogleOptions();
            configuration.Bind(GoogleOptions.Name, googleOptions);

            if (!googleOptions.Enable)
                return;

            authenticationBuilder.AddJwtBearer(MultiSchemeAuthenticationExtensions.GoogleScheme, options =>
            {
                options.UseGoogle(clientId: googleOptions.ClientId ?? string.Empty);
                options.Events = new CustomJwtBearerEvents(logger);
            });
        }
    }
}
