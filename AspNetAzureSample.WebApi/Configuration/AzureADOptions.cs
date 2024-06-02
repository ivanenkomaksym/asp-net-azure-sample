namespace AspNetAzureSample.Configuration
{
    public record AzureADOptions
    {
        public static readonly string Name = "AzureAd";

        public bool Enable { get; set; }
        public string? Domain { get; set; }
        public string? ClientID { get; set; }
        public string? Audience { get; set; }
        public string? Instance { get; set; }
        public string? TenantId { get; set; }
        public IEnumerable<string>? AcceptedTenantIds { get; set; } 
        public string? RoleName { get; set; }
    }
}
