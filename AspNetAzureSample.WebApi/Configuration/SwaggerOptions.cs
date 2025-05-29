namespace AspNetAzureSample.Configuration
{
    public enum SwaggerSecuritySchemeType
    {
        Azure,
        Auth0
    }

    public record SwaggerOptions
    {
        public static readonly string Name = "Swagger";

        public string? ClientID { get; set; }
        public string? ClientSecret { get; set; }
        public string? RedirectUrl { get; set; }
        public SwaggerSecuritySchemeType SecuritySchemeType { get; set; } = SwaggerSecuritySchemeType.Azure;
    }
}
