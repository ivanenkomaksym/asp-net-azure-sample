using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace AspNetAzureSample.Authentication
{
    public interface ISwaggerConfigurator
    {
        public OpenApiOAuthFlow BuildImplicitFlow();

        public void ConfigureSwaggerUIOptions(SwaggerUIOptions swaggerUIOptions);
    }
}
