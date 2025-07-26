using System.Net;
using System.Text.Json;

namespace AspNetAzureSample.Tests;

// Use IClassFixture to share the factory instance across tests in the class
public class GymMembershipControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    public GymMembershipControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient(); // Creates an HttpClient configured for your test server
    }

    [Fact]
    public async Task GetMembershipDetails_ReturnsOk_WhenUserMeetsMinimumAge()
    {
        _client.DefaultRequestHeaders.Add("age", "25");
        var response = await _client.GetAsync("/api/GymMembership/details");

        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        var content = await response.Content.ReadAsStringAsync();
        var membershipDetails = JsonSerializer.Deserialize<JsonElement>(content);

        Assert.Equal("testuser", membershipDetails.GetProperty("userId").GetString());
    }
}