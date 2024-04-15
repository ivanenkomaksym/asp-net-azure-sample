using AspNetAzureSample.Common;
using AspNetAzureSample.Security;
using System.Globalization;
using System.Security.Claims;

namespace AspNetAzureSample.Extensions
{
    public static class ClaimsPrincipalExtensions
    {
        public static string FindFirstValue(this ClaimsPrincipal principal, string claimType, bool throwIfNotFound = false)
        {
            Guard.ArgumentNotNull(principal, nameof(principal));

            var value = principal.FindFirst(claimType)?.Value;
            if (throwIfNotFound && string.IsNullOrWhiteSpace(value))
            {
                throw new InvalidOperationException(
                    string.Format(CultureInfo.InvariantCulture, "The supplied principal does not contain a claim of type {0}", claimType));
            }

            return value;
        }

        public static string GetIssuerValue(this ClaimsPrincipal principal, bool throwIfNotFound = true)
        {
            return principal.FindFirstValue(OpenIdConnectClaimTypes.IssuerValue, throwIfNotFound);
        }

        /// <summary>
        /// Extension method on <see cref="System.Security.Claims.ClaimsPrincipal"/> which returns the AAD Tenant ID, if it exists.
        /// </summary>
        /// <param name="principal">A <see cref="System.Security.Claims.ClaimsPrincipal"/> representing the currently signed in ASP.NET user.</param>
        /// <returns>The AAD Tenant ID if it exists, otherwise, an exception is thrown.</returns>
        public static string GetTenantIdValue(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(AzureADClaimTypes.TenantId, true);
        }

        public static string GetObjectIdentifierValue(this ClaimsPrincipal principal, bool throwIfNotFound = false)
        {
            return principal.FindFirstValue(AzureADClaimTypes.ObjectId, throwIfNotFound);
        }

        public static string GetScopeValue(this ClaimsPrincipal principal, bool throwIfNotFound = false)
        {
            return principal.FindFirstValue(AzureADClaimTypes.Scope, throwIfNotFound);
        }

        public static string GetRoleValue(this ClaimsPrincipal principal, bool throwIfNotFound = false)
        {
            return principal.FindFirstValue(AzureADClaimTypes.Role, throwIfNotFound);
        }

        public static string GetRolesValue(this ClaimsPrincipal principal, bool throwIfNotFound = false)
        {
            return principal.FindFirstValue(AzureADClaimTypes.Roles, throwIfNotFound);
        }

        public static string GetDisplayNameValue(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(AzureADClaimTypes.Name, true);
        }

        public static string GetEmailValue(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimTypes.Email, true);
        }

        public static string GetUserName(this ClaimsPrincipal principal)
        {
            return principal.FindFirstValue(ClaimsIdentity.DefaultNameClaimType);
        }

        public static bool IsSignedInToApplication(this ClaimsPrincipal principal)
        {
            Guard.ArgumentNotNull(principal, nameof(principal));
            return principal.Identity != null && principal.Identity.IsAuthenticated;
        }
    }
}
