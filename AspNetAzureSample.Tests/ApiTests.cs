using System.Net.Http.Headers;
using AspNetAzureSample.Tests.Tokens;
using Microsoft.AspNetCore.Mvc.Testing;

namespace AspNetAzureSample.Tests
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        public HttpClient HttpClient;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            HttpClient = factory.CreateClient();
        }

        [Fact]
        public async Task GetWeatherForecastWithAccessToken()
        {
            var tokenResponse = await TokenExtensions.GetTokenAsync();
            HttpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", tokenResponse.access_token);

            var response = await HttpClient.GetAsync("/WeatherForecast");
            var responseCode = response.StatusCode.ToString();
            Assert.Equal("OK", responseCode);
        }
    }
}
