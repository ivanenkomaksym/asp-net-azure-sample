namespace AspNetAzureSample.Security
{
    public static class AzureADClaimTypes
    {
        public const string TenantId = "http://schemas.microsoft.com/identity/claims/tenantid";
        public const string ObjectId = "http://schemas.microsoft.com/identity/claims/objectidentifier";
        public const string Scope = "http://schemas.microsoft.com/identity/claims/scope";
        public const string Role = "http://schemas.microsoft.com/ws/2008/06/identity/claims/role";
        public const string Name = "name";
    }
}
