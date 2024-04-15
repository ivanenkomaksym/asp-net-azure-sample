using Microsoft.IdentityModel.Tokens;

namespace AspNetAzureSample.Validation
{
    public static class CustomIssuerValidator
    {
        public static string ValidateSpecificIssuers(string issuer,
                                                     SecurityToken securityToken,
                                                     TokenValidationParameters validationParameters,
                                                     IEnumerable<string> validIssuers)
        {
            if (validIssuers.Any(item => issuer.Contains(item)))
            {
                return issuer;
            }
            else
            {
                throw new SecurityTokenInvalidIssuerException("The sign-in user's account does not belong to one of the tenants that this Web App accepts users from.");
            }
        }
    }
}
