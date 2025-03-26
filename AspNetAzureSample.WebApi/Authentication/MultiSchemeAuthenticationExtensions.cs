using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.IdentityModel.Tokens.Jwt;

namespace AspNetAzureSample.Authentication
{
    public static class MultiSchemeAuthenticationExtensions
    {
        public static readonly string GoogleScheme = "Google";
        public static readonly string AzureOrGoogleOrAuth0AuthScheme = "Azure_OR_Google_OR_Auth0_OR_Cookie";
        public static readonly string Auth0Scheme = "Auth0";

        /// <summary>
        /// Used to select a default scheme for the current request that authentication handlers should forward all authentication operations to by default
        /// Checks token's issuer and based on that select corresponding authentication scheme to challenge.
        /// <see cref="https://learn.microsoft.com/en-us/aspnet/core/security/authorization/limitingidentitybyscheme?view=aspnetcore-8.0#use-multiple-authentication-schemes"/>
        /// Falls back to cookie-based authentication in case tokens are not used.
        /// </summary>
        /// <param name="options">Contains the options used by PolicySchemeHandler.</param>
        public static void SelectDefaultSchemeForCurrentRequest(this PolicySchemeOptions options)
        {
            options.ForwardDefaultSelector = context =>
            {
                string? token = context.Request.Headers.Authorization.FirstOrDefault()?.Split(' ').Last();

                if (!string.IsNullOrEmpty(token))
                {
                    var handler = new JwtSecurityTokenHandler();
                    var jwtToken = handler.ReadToken(token) as JwtSecurityToken;

                    if (jwtToken != null)
                    {
                        string issuer = jwtToken.Issuer;

                        if (issuer.StartsWith("https://login.microsoftonline.com/"))
                            return JwtBearerDefaults.AuthenticationScheme;
                        else if (issuer == "https://accounts.google.com")
                            return GoogleScheme;
                        else if (issuer.Contains("auth0.com"))
                            return Auth0Scheme;
                    }
                }

                if (context.Request.Cookies.ContainsKey(".AspNetCore.Identity.Application"))
                    return "Identity.BearerAndApplication";

                return JwtBearerDefaults.AuthenticationScheme;
            };
        }
    }
}
