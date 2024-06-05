using System.Text.Json;
using AspNetAzureSample.Configuration;
using Microsoft.Extensions.Configuration;

namespace AspNetAzureSample.Tests.Tokens
{
    public static class TokenExtensions
    {
        public static async Task<TokenResponse> GetTokenAsync()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            var builder = new ConfigurationBuilder().AddJsonFile(configPath);

            var configuration = builder.Build();

            var azureadOptions = new AzureADOptions();
            configuration.Bind(AzureADOptions.Name, azureadOptions);

            if (azureadOptions.Client == null)
                throw new ArgumentException($"{ClientOptions.Name} must be filled in.");

            var tenant = azureadOptions.TenantId;
            var clientId = azureadOptions.Client.ClientID;
            var clientSecret = azureadOptions.Client.ClientSecret;
            var scope = $"{azureadOptions.ClientID}/.default";

            if (clientSecret == null || scope == null)
                throw new ArgumentException("");

            var tokenEndpoint = $"https://login.microsoftonline.com/{tenant}/oauth2/v2.0/token";

            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("scope", scope),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "client_credentials")
            });

            var request = new HttpRequestMessage(HttpMethod.Post, tokenEndpoint)
            {
                Content = requestBody
            };

            var client = new HttpClient();
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(responseBody);
            if (tokenResponse == null)
                throw new Exception();

            return tokenResponse;
        }
    }
}
