namespace AspNetAzureSample.Configuration
{
    public class Auth0Options
    {
        public static readonly string Name = "Auth0";

        public bool Enable { get; set; }
        public string Authority { get; set; } = string.Empty;
        public string Audience { get; set; } = string.Empty;
    }
}
