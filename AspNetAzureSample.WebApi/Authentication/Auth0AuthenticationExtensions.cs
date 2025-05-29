using AspNetAzureSample.Configuration;
using AspNetAzureSample.Validation;
using Microsoft.AspNetCore.Authentication;

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
            });
        }
    }
}
