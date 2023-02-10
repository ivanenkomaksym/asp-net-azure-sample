using Microsoft.Identity.Web;

namespace AspNetAzureSample.Security
{
    public static class AzureADClaimTypes
    {
        public const string TenantId = ClaimConstants.TenantId;
        public const string ObjectId = ClaimConstants.ObjectId;
        public const string Scope = ClaimConstants.Scope;
        public const string Role = ClaimConstants.Role;
        public const string Roles = ClaimConstants.Roles;
        public const string Name = ClaimConstants.Name;
    }
}
