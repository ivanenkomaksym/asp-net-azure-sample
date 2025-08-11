using System.Net;
using System.Security.Claims;
using System.Text.Json;

namespace AspNetAzureSample.Tests;

// Use IClassFixture to share the factory instance across tests in the class
public class CycleManagementControllerTests : IClassFixture<CustomWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly CustomWebApplicationFactory _factory;

    private const string RequestUri = "/Maintenance/InitializeCycle";

    public CycleManagementControllerTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    [Fact]
    public async Task InitializeCycle_WithoutAnyScope_ReturnsForbidden()
    {
        // Act
        var response = await _client.PostAsync(RequestUri, null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonDocument.Parse(responseBody);
        var root = jsonResponse.RootElement;

        Assert.Equal("AuthorizationFailed", root.GetProperty("error").GetString());
        Assert.Contains("Forbidden. Current user does not have a specified scope", root.GetProperty("message").GetString());
        Assert.Contains("Requirement Failed", root.GetProperty("details").GetString());
    }

    [Fact]
    public async Task InitializeCycle_WithoutRequiredScope_ReturnsForbidden()
    {
        // Arrange
        // 1. Create a user principal with a different scope.
        var claims = new[]
        {
            new Claim("scope", "some-other-scope")
        };
        TestAuthHandler.ExtraClaims = claims;

        // Act
        var response = await _client.PostAsync(RequestUri, null);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);

        var responseBody = await response.Content.ReadAsStringAsync();

        var jsonResponse = JsonDocument.Parse(responseBody);
        var root = jsonResponse.RootElement;

        Assert.Equal("AuthorizationFailed", root.GetProperty("error").GetString());
        Assert.Contains("Forbidden. Current user does not have a specified scope", root.GetProperty("message").GetString());
        Assert.Contains("Requirement Failed", root.GetProperty("details").GetString());
    }

    [Fact]
    public async Task InitializeCycle_WithRequiredScope_ReturnsOk()
    {
        // Arrange
        var requiredScope = CustomWebApplicationFactory.MaintenanceScope;

        // 1. Create a user principal with the correct scope claim.
        var claims = new[]
        {
            new Claim("scope", requiredScope) // The crucial claim for this test
        };
        TestAuthHandler.ExtraClaims = claims;

        // Act
        var response = await _client.PostAsync(RequestUri, null);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}