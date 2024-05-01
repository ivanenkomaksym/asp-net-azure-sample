using AspNetAzureSample.Configuration;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace AspNetAzureSample.Security
{
    /// <summary>
    /// Mocking Authentication and Authorization in ASP.NET Core Integration Tests
    /// https://mazeez.dev/posts/auth-in-integration-tests
    /// </summary>
    public class FixedAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public FixedAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
                                          ILoggerFactory logger,
                                          UrlEncoder encoder,
                                          IConfiguration configuration)
            : base(options, logger, encoder)
        {
            Configuration = configuration;
        }

        public const string AuthenticationScheme = "FixedAuthentication";

        private readonly IConfiguration Configuration;

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new List<Claim> { };

            var azureadOptions = new AzureADOptions();
            Configuration.Bind(AzureADOptions.Name, azureadOptions);

            if (azureadOptions.RoleName != null)
                claims.Add(new Claim(ClaimTypes.Role, azureadOptions.RoleName));

            var identity = new ClaimsIdentity(claims, AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, AuthenticationScheme);

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
