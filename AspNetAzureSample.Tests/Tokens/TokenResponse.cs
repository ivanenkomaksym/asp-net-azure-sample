namespace AspNetAzureSample.Tests.Tokens
{
    public class TokenResponse
    {
        public string token_type { get; set; }
        public ulong expires_in { get; set; }
        public string access_token { get; set; }
    }
}
