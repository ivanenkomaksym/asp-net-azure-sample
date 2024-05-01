using System.IdentityModel.Tokens.Jwt;

namespace AspNetAzureSample.UserProviders
{
    /// <summary>
    /// Can be used in the following scenario:
    /// 1. WebApi has AzureAd authentication disabled and doesn't actively check authorization header
    /// 2. Client's token is already authenticated by the upstream service
    /// 3. Client still sends bearer token through
    /// 4. WebApi can still read user name from the bearer token assuming it's been already validated
    /// First tries to retrive user name from bearer token, otherwise falls back to <see cref="DefaultUserProvider"/>.
    /// </summary>
    public class BearerTokenUserProvider : DefaultUserProvider
    {
        public BearerTokenUserProvider(ILogger<DefaultUserProvider> logger)
            : base(logger)
        {
        }

        public override string GetUserName(HttpContext httpContext)
        {
            var authorizationHeader = httpContext.Request.Headers["Authorization"].ToString();
            if (!authorizationHeader.Contains("Bearer"))
                return base.GetUserName(httpContext);

            var encoded = authorizationHeader.Substring(7); // Skip "Bearer "
            var token = new JwtSecurityToken(encoded);

            var nameClaim = token.Claims.FirstOrDefault(claim => claim.Type == "unique_name", null);
            var emailClaim = token.Claims.FirstOrDefault(claim => claim.Type == "email", null);

            if (nameClaim != null)
            {
                var ipaddrClaim = token.Claims.FirstOrDefault(claim => claim.Type == "ipaddr", null);

                Logger.BearerTokenUserProviderUserNameReceived(nameClaim.Value, ipaddrClaim);

                return nameClaim.Value;
            }
            else if (emailClaim != null)
            {
                Logger.BearerTokenUserProviderEmailReceived(emailClaim.Value);

                return emailClaim.Value;
            }
            else
            {
                return base.GetUserName(httpContext);
            }
        }
    }
}
