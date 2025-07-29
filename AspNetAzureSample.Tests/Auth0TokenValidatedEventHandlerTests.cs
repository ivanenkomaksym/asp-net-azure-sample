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
    private const string MissingOrganizationMessage = "Required organization ID is not set in configuration.";

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

    // Helper to create a TokenValidatedContext
    private TokenValidatedContext CreateTokenValidatedContext(
        ClaimsPrincipal principal,
        IConfiguration configuration)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.RequestServices = NSubstitute.Substitute.For<IServiceProvider>(); // Minimal services needed for context

        // Mock the AuthenticationScheme. It's often required for the context.
        var authScheme = new AuthenticationScheme(
            MultiSchemeAuthenticationExtensions.Auth0Scheme,
            MultiSchemeAuthenticationExtensions.Auth0Scheme,
            typeof(JwtBearerHandler) // Can be any handler type for mocking purposes
        );

        var authenticationTicket = new AuthenticationTicket(principal, authScheme.Name);

        // TokenValidatedContext constructor: HttpContext, AuthenticationScheme, Options, SecurityToken, ClaimsPrincipal, AuthenticationTicket
        var context = new TokenValidatedContext(
            httpContext,
            authScheme,
            new JwtBearerOptions()
        );

        context.Principal = principal;
        // Since your OnTokenValidated handler directly references builder.Configuration,
        // we'll store the configuration in the HttpContext.Items for the test,
        // or more cleanly, pass it to the options event setup.
        // For testing, we'll mimic how it's closed over in Program.cs
        // by making sure the configuration is available when the event handler is created.

        return context;
    }

    // Helper method that simulates the JwtBearerEvents setup from Program.cs
    // and returns the OnTokenValidated delegate.
    private Func<TokenValidatedContext, Task> GetOnTokenValidatedDelegate(IConfiguration configuration)
    {
        var options = new JwtBearerOptions();

        // This part mimics your Program.cs setup of options.Events
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = context =>
            {
                var requiredOrgId = configuration["Auth0:OrganizationId"]; // Access the config here

                var organizationIdClaim = context.Principal?.FindFirst(Claims.OrganizationIdClaimType);

                if (string.IsNullOrEmpty(requiredOrgId) || organizationIdClaim == null)
                {
                    context.Fail(MissingOrganizationMessage);
                    return Task.CompletedTask;
                }

                if (organizationIdClaim.Value != requiredOrgId)
                    context.Fail($"Token does not contain the required organization ID claim or its value is incorrect. Required: '{requiredOrgId}'");

                return Task.CompletedTask;
            }
        };
        return options.Events.OnTokenValidated;
    }

    private Func<JwtBearerChallengeContext, Task> GetOnChallengeDelegate()
    {
        var options = new JwtBearerOptions
        {
            // This part mimics your Program.cs setup of options.Events.OnChallenge
            Events = new JwtBearerEvents
            {
                OnChallenge = context =>
                {
                    if (context.AuthenticateFailure != null)
                    {
                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                        context.Response.ContentType = "application/json";
                        var errorResponse = new
                        {
                            status = StatusCodes.Status401Unauthorized,
                            message = context.AuthenticateFailure.Message
                        };
                        return context.Response.WriteAsync(JsonSerializer.Serialize(errorResponse));
                    }
                    return Task.CompletedTask;
                }
            }
        };
        return options.Events.OnChallenge;
    }

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

        var context = CreateTokenValidatedContext(principal, configuration);
        var onTokenValidated = GetOnTokenValidatedDelegate(configuration);

        // Act
        await onTokenValidated(context);

        // Assert
        Assert.Null(context.Result); // A null result means no failure was explicitly set
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
            new Claim(Claims.OrganizationIdClaimType, IncorrectOrgId) // Incorrect ID
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));

        var context = CreateTokenValidatedContext(principal, configuration);
        var onTokenValidated = GetOnTokenValidatedDelegate(configuration);

        // Act
        await onTokenValidated(context);

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
            new Claim(ClaimTypes.NameIdentifier, "user123") // Missing org_id claim
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));

        var context = CreateTokenValidatedContext(principal, configuration);
        var onTokenValidated = GetOnTokenValidatedDelegate(configuration);

        // Act
        await onTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        Assert.Equal(MissingOrganizationMessage, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnTokenValidated_WhenRequiredOrgIdIsNull_FailsIfClaimExists()
    {
        // Arrange
        var configuration = CreateMockConfiguration(null); // Required Org ID is null
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123"),
            new Claim(Claims.OrganizationIdClaimType, TestAuth0OrganizationId) // Claim is present
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));

        var context = CreateTokenValidatedContext(principal, configuration);
        var onTokenValidated = GetOnTokenValidatedDelegate(configuration);

        // Act
        await onTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        // The message should reflect that requiredOrgId was null/empty and claim existed
        Assert.Equal(MissingOrganizationMessage, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnTokenValidated_WhenRequiredOrgIdIsEmpty_FailsIfClaimExists()
    {
        // Arrange
        var configuration = CreateMockConfiguration(string.Empty); // Required Org ID is empty
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123"),
            new Claim(Claims.OrganizationIdClaimType, TestAuth0OrganizationId) // Claim is present
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));

        var context = CreateTokenValidatedContext(principal, configuration);
        var onTokenValidated = GetOnTokenValidatedDelegate(configuration);

        // Act
        await onTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        Assert.Equal(MissingOrganizationMessage, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnTokenValidated_WhenRequiredOrgIdIsNullAndClaimIsMissing_Fails()
    {
        // Arrange
        var configuration = CreateMockConfiguration(null); // Required Org ID is null
        var principal = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "user123") // Claim is missing
        }, MultiSchemeAuthenticationExtensions.Auth0Scheme));

        var context = CreateTokenValidatedContext(principal, configuration);
        var onTokenValidated = GetOnTokenValidatedDelegate(configuration);

        // Act
        await onTokenValidated(context);

        // Assert
        Assert.NotNull(context.Result);
        Assert.NotNull(context.Result.Failure);
        Assert.Equal(MissingOrganizationMessage, context.Result.Failure.Message);
    }

    [Fact]
    public async Task OnChallenge_WhenAuthenticationFails_ReturnsCustomJsonResponse()
    {
        // Arrange
        var mockHttpContext = new DefaultHttpContext();
        // Setup a MemoryStream for the response body to capture the output
        var responseBodyStream = new MemoryStream();
        mockHttpContext.Response.Body = responseBodyStream;
        mockHttpContext.Response.ContentType = "text/plain"; // Initial content type

        var testFailureMessage = "Token does not contain the required organization ID claim or its value is incorrect. Required: 'some-org-id'";
        var authenticateFailureException = new Exception(testFailureMessage);

        // Mock the AuthenticationScheme
        var authScheme = new AuthenticationScheme(
            MultiSchemeAuthenticationExtensions.Auth0Scheme,
            MultiSchemeAuthenticationExtensions.Auth0Scheme,
            typeof(JwtBearerHandler)
        );

        // Create a ChallengeContext instance. Its constructor is internal, so we need Moq.
        var mockChallengeContext = Substitute.For<JwtBearerChallengeContext>(
            mockHttpContext,
            authScheme,
            new JwtBearerOptions(),
            new AuthenticationProperties()
        );

        // Set up the properties that your OnChallenge handler expects
        mockChallengeContext.AuthenticateFailure = authenticateFailureException;
        //mockChallengeContext.Response.Returns(mockHttpContext.Response);

        var onChallenge = GetOnChallengeDelegate();

        // Act
        await onChallenge(mockChallengeContext);

        // Assert
        Assert.Equal(StatusCodes.Status401Unauthorized, mockHttpContext.Response.StatusCode);
        Assert.Equal("application/json", mockHttpContext.Response.ContentType);

        // Read the response body
        responseBodyStream.Seek(0, SeekOrigin.Begin); // Rewind stream to read from beginning
        using var reader = new StreamReader(responseBodyStream);
        var responseBody = await reader.ReadToEndAsync();

        // Deserialize and check the JSON content
        var jsonResponse = JsonSerializer.Deserialize<JsonElement>(responseBody);
        Assert.Equal(StatusCodes.Status401Unauthorized, jsonResponse.GetProperty("status").GetInt32());
        Assert.Equal(testFailureMessage, jsonResponse.GetProperty("message").GetString());
    }
}
