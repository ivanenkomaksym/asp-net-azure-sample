namespace AspNetAzureSample.Configuration
{
    public class GoogleOptions
    {
        public static readonly string Name = "Google";

        public bool Enable { get; set; }
        public string? ClientId { get; set; }
    }
}
