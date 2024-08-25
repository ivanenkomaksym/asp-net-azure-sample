using AspNetAzureSample.Authentication;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace AspNetAzureSample.Tests
{
    public class AuthenticatedTestRequestMiddlewareTests
    {
        [Fact]
        public async Task Invoke_WithTestingHeaders_SetsHttpContextUser()
        {
            // Arrange
            var context = new DefaultHttpContext();
            var headers = context.Request.Headers;

            // Add testing headers
            headers.Append(AuthenticatedTestRequestMiddleware.TestingNameHeader, "TestUser");
            headers.Append(AuthenticatedTestRequestMiddleware.TestingNameIdHeader, "12345");

            var middleware = new AuthenticatedTestRequestMiddleware((HttpContext context) => Task.CompletedTask);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.NotNull(context.User);
            Assert.True(context.User.Identity?.IsAuthenticated);
            Assert.Equal(AuthenticatedTestRequestMiddleware.TestingAuthenticationScheme, context.User.Identity?.AuthenticationType);
            Assert.Equal("TestUser", context.User.Identity?.Name);
            Assert.Equal("12345", context.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
        }

        [Fact]
        public async Task Invoke_WithoutTestingHeaders_DoesNotSetHttpContextUser()
        {
            // Arrange
            var context = new DefaultHttpContext();

            var middleware = new AuthenticatedTestRequestMiddleware((HttpContext context) => Task.CompletedTask);

            // Act
            await middleware.Invoke(context);

            // Assert
            Assert.Null(context.User.Identity?.Name);
            Assert.False(context.User.Identity?.IsAuthenticated);
        }
    }
}
