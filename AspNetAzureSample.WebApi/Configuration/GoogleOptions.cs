namespace AspNetAzureSample.Configuration
{
    public class GoogleOptions
    {
        public static readonly string Name = "Google";

        public bool Enable { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;

        /// <summary>
        /// Non-expiring refresh token that should be retrieved from Google's OAuth 2.0 Playground.
        /// </summary>
        /// <remarks>Never do this in production.</remarks>
        public string RefreshToken { get; set; } = string.Empty;
    }
}
