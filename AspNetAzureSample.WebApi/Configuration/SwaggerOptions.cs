namespace AspNetAzureSample.Configuration
{
    public record SwaggerOptions
    {
        public static readonly string Name = "Swagger";

        public string? ClientID { get; set; }
        public string? ClientSecret { get; set; }
        public string? RedirectUrl { get; set; }
    }
}
