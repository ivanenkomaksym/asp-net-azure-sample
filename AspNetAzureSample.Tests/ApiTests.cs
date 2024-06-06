using System.Net.Http.Headers;
using System.Text;
using AspNetAzureSample.Tests.Tokens;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AspNetAzureSample.Tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient HttpClient;
        private readonly string LoginURI = "http://localhost/login?useCookies=true";
        private readonly string WeatherForecastURI = "http://localhost/WeatherForecast";

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            HttpClient = factory.CreateClient(new WebApplicationFactoryClientOptions
            {
                HandleCookies = true
            });
        }

        [Fact]
        public async Task GetWeatherForecastWithAccessToken()
        {
            // Arrange
            var tokenResponse = await TokenExtensions.GetTokenAsync();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, tokenResponse.access_token);

            // Act
            var response = await HttpClient.GetAsync(WeatherForecastURI);
            var responseCode = response.StatusCode;

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, responseCode);
        }

        [Fact]
        public async Task GetWeatherForecastWithInvalidAccessTokenShouldFail()
        {
            // Arrange
            var accessToken = TokenExtensions.GenerateJwtTokenWithoutSignature();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, accessToken);

            // Act
            var response = await HttpClient.GetAsync(WeatherForecastURI);
            var responseCode = response.StatusCode;

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.Unauthorized, responseCode);
            Assert.True(response.Headers.Contains("WWW-Authenticate"));
            Assert.Contains("The signature key was not found", response.Headers.GetValues("WWW-Authenticate").FirstOrDefault());
        }

        [Fact]
        public async Task GetWeatherForecastWithGoogleIdToken()
        {
            // Arrange
            var tokenResponse = await TokenExtensions.GetGoogleTokenAsync();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue(JwtBearerDefaults.AuthenticationScheme, tokenResponse.id_token);

            // Act
            var response = await HttpClient.GetAsync(WeatherForecastURI);
            var responseCode = response.StatusCode;

            // Assert
            Assert.Equal(System.Net.HttpStatusCode.OK, responseCode);
        }

        [Fact]
        public async Task GetWeatherForecastWithCookieAuthn()
        {
            // Arrange
            var email = "admin@example.com";
            var password = "P@ssword1";

            var payload = new
            {
                email,
                password
            };

            var jsonPayload = Newtonsoft.Json.JsonConvert.SerializeObject(payload);
            var content = new StringContent(jsonPayload, Encoding.UTF8, "application/json");

            // Act
            var loginResponse = await HttpClient.PostAsync(LoginURI, content);
            loginResponse.EnsureSuccessStatusCode();

            Assert.True(loginResponse.Headers.Contains("Set-Cookie"));

            var weatherForecastResponse = await HttpClient.GetAsync(WeatherForecastURI);
            weatherForecastResponse.EnsureSuccessStatusCode();
        }
    }
}
