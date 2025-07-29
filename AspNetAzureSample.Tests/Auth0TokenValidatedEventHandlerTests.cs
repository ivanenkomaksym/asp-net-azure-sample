using AspNetAzureSample.Authentication;
using AspNetAzureSample.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NSubstitute;
using System.Security.Claims;
using System.Text.Json;

namespace AspNetAzureSample.Tests;

public class Auth0TokenValidatedEventHandlerTests
{
    private const string TestAuth0OrganizationId = "test-org-123";
    private const string IncorrectOrgId = "wrong-org-456";

    // Helper to create a mock IConfiguration for the test
    private IConfiguration CreateMockConfiguration(string auth0OrgIdValue)
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string>
            {
                {"Auth0:OrganizationId", auth0OrgIdValue}
            })
            .Build();
        return configuration;
    }

    private TokenValidatedContext CreateTokenValidatedContext(ClaimsPrincipal principal)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = NSubstitute.Substitute.For<IServiceProvider>();
        var authScheme = new AuthenticationScheme(
            MultiSchemeAuthenticationExtensions.Auth0Scheme,
            MultiSchemeAuthenticationExtensions.Auth0Scheme,
            typeof(JwtBearerHandler)
        );
        var context = new TokenValidatedContext(
            httpContext,
            authScheme,
            new JwtBearerOptions()
        );
        context.Principal = principal;
        return context;
    }

    private JwtBearerChallengeContext CreateChallengeContext(HttpContext httpContext, Exception? failure = null)
    {
        var authScheme = new AuthenticationScheme(
            MultiSchemeAuthenticationExtensions.Auth0Scheme,
            MultiSchemeAuthenticationExtensions.Auth0Scheme,
            typeof(JwtBearerHandler)
        );
        var challengeContext = Substitute.For<JwtBearerChallengeContext>(
            httpContext,
            authScheme,
            new JwtBearerOptions(),
            new AuthenticationProperties()
        );
        challengeContext.AuthenticateFailure = failure;
        return challengeContext;
    }

    private JwtBearerEvents CreateEvents(IConfiguration configuration)
    {
        var options = new JwtBearerOptions();

        // This part mimics your Program.cs setup of options.Events
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var requiredOrgId = configuration["Auth0:OrganizationId"]; // Access the config here
                return Auth0AuthenticationExtensions.ValidateToken(context, requiredOrgId);
            },
            OnChallenge = Auth0AuthenticationExtensions.CustomizeResponse
        };
        return options.Events;
    }

    // ...existing code...

    [Fact]
    public async Task OnTokenValidated_WithCorrectOrganizationIdClaim_Succeeds()
    {
        // Arrange
        var configuration = CreateMockConfiguration(TestAuth0OrganizationId);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123"),
            new Claim(Claims.OrganizationIdClaimType, TestAuth0OrganizationId)
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));
        var context = CreateTokenValidatedContext(principal);
        var events = CreateEvents(configuration);

        // Act
        await events.OnTokenValidated(context);

        // Assert
        Assert.Null(context.Result);
        Assert.True(context.Principal?.Identity?.IsAuthenticated);
    }

    [Fact]
    public async Task OnTokenValidated_WithIncorrectOrganizationIdClaim_Fails()
    {
        // Arrange
        var configuration = CreateMockConfiguration(TestAuth0OrganizationId);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123"),
            new Claim(Claims.OrganizationIdClaimType, IncorrectOrgId)
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));
        var context = CreateTokenValidatedContext(principal);
        var events = CreateEvents(configuration);

        // Act
        await events.OnTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        Assert.Contains("incorrect", context.Result.Failure.Message);
        Assert.Contains(TestAuth0OrganizationId, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnTokenValidated_WithMissingOrganizationIdClaim_Fails()
    {
        // Arrange
        var configuration = CreateMockConfiguration(TestAuth0OrganizationId);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123")
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));
        var context = CreateTokenValidatedContext(principal);
        var events = CreateEvents(configuration);

        // Act
        await events.OnTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        Assert.Equal(Auth0AuthenticationExtensions.MissingOrganizationMessage, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnTokenValidated_WhenRequiredOrgIdIsNull_FailsIfClaimExists()
    {
        // Arrange
        var configuration = CreateMockConfiguration(null);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123"),
            new Claim(Claims.OrganizationIdClaimType, TestAuth0OrganizationId)
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));
        var context = CreateTokenValidatedContext(principal);
        var events = CreateEvents(configuration);

        // Act
        await events.OnTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        Assert.Equal(Auth0AuthenticationExtensions.MissingOrganizationMessage, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnTokenValidated_WhenRequiredOrgIdIsEmpty_FailsIfClaimExists()
    {
        // Arrange
        var configuration = CreateMockConfiguration(string.Empty);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123"),
            new Claim(Claims.OrganizationIdClaimType, TestAuth0OrganizationId)
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));
        var context = CreateTokenValidatedContext(principal);
        var events = CreateEvents(configuration);

        // Act
        await events.OnTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        Assert.Equal(Auth0AuthenticationExtensions.MissingOrganizationMessage, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnTokenValidated_WhenRequiredOrgIdIsNullAndClaimIsMissing_Fails()
    {
        // Arrange
        var configuration = CreateMockConfiguration(null);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123")
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));
        var context = CreateTokenValidatedContext(principal);
        var events = CreateEvents(configuration);

        // Act
        await events.OnTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        Assert.Equal(Auth0AuthenticationExtensions.MissingOrganizationMessage, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnChallenge_WhenAuthenticationFails_ReturnsCustomJsonResponse2()
    {
        // Arrange
        var configuration = CreateMockConfiguration(string.Empty);
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123"),
            new Claim(Claims.OrganizationIdClaimType, TestAuth0OrganizationId)
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));
        var context = CreateTokenValidatedContext(principal);
        var events = CreateEvents(configuration);
        var testFailureMessage = "Token does not contain the required organization ID claim or its value is incorrect. Required: 'some-org-id'";
        var authenticateFailureException = new Exception(testFailureMessage);

        var mockHttpContext = new DefaultHttpContext();
        var responseBodyStream = new MemoryStream();
        mockHttpContext.Response.Body = responseBodyStream;
        mockHttpContext.Response.ContentType = "text/plain";
        var challengeContext = CreateChallengeContext(mockHttpContext, authenticateFailureException);

        // Act
        await events.OnTokenValidated(context);
        await events.OnChallenge(challengeContext);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, mockHttpContext.Response.StatusCode);
        Assert.Equal("application/json", mockHttpContext.Response.ContentType);
        responseBodyStream.Seek(0, SeekOrigin.Begin);
        using var reader = new StreamReader(responseBodyStream);
        var responseBody = await reader.ReadToEndAsync();
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
        Assert.Equal(StatusCodes.Status401Unauthorized, jsonResponse.GetProperty("status").GetInt32());
        Assert.Equal(testFailureMessage, jsonResponse.GetProperty("message").GetString());
    }
}
