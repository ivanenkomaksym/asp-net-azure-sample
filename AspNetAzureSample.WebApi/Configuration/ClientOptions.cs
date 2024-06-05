namespace AspNetAzureSample.Configuration
{
    public class ClientOptions
    {
        public static readonly string Name = "Client";

        public required string ClientID { get; set; }
        public required string ClientSecret { get; set; }
    }
}
