using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using AspNetAzureSample.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;

namespace AspNetAzureSample.Tests.Tokens
{
    /// <summary>
    /// Extension methods to retrieve tokens.
    /// TODO: it deserves to be injected as a resource and work around Configuration, instead of building it on the fly.
    /// </summary>
    public static class TokenExtensions
    {
        public static async Task<TokenResponse> GetTokenAsync()
        {
            var azureadOptions = GetAzureADOptions();

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
                throw new Exception("Failed to deserialize response body.");

            return tokenResponse;
        }

        public static async Task<TokenResponse> GetGoogleTokenAsync()
        {
            var googleOptions = GetGoogleOptions() ?? throw new ArgumentException($"{GoogleOptions.Name} must be filled in.");
            var clientId = googleOptions.ClientId;
            var clientSecret = googleOptions.ClientSecret;
            var refreshToken = googleOptions.RefreshToken;

            var tokenEndpoint = "https://oauth2.googleapis.com/token";

            var requestBody = new FormUrlEncodedContent(new[]
            {
                new KeyValuePair<string, string>("client_id", clientId),
                new KeyValuePair<string, string>("refresh_token", refreshToken),
                new KeyValuePair<string, string>("client_secret", clientSecret),
                new KeyValuePair<string, string>("grant_type", "refresh_token")
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
                throw new Exception("Failed to deserialize response body.");

            return tokenResponse;
        }

        public static string GenerateJwtTokenWithoutSignature()
        {
            var azureadOptions = GetAzureADOptions();
            var userId = "admin@example.com";
            var issuer = $"https://login.microsoftonline.com/{azureadOptions.TenantId}";
            var audience = azureadOptions.Audience;
            var secretKey = "chR8D4FlG6E3mwquM3VtnsugS6zqsrBQ";

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience),
                new Claim(ClaimTypes.NameIdentifier, userId)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: issuer,
                audience: audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(30), // Token expiration time
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private static AzureADOptions GetAzureADOptions()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            var builder = new ConfigurationBuilder().AddJsonFile(configPath);

            var configuration = builder.Build();

            var azureadOptions = new AzureADOptions();
            configuration.Bind(AzureADOptions.Name, azureadOptions);

            return azureadOptions;
        }

        private static GoogleOptions GetGoogleOptions()
        {
            var projectDir = Directory.GetCurrentDirectory();
            var configPath = Path.Combine(projectDir, "appsettings.json");

            var builder = new ConfigurationBuilder().AddJsonFile(configPath);

            var configuration = builder.Build();

            var googleOptions = new GoogleOptions();
            configuration.Bind(GoogleOptions.Name, googleOptions);

            return googleOptions;
        }
    }
}
