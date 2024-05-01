using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using AspNetAzureSample.UserProviders;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NSubstitute;

namespace AspNetAzureSample.Tests
{
    public class BearerTokenUserProviderTests
    {
        [Fact]
        public void GetUserName_WithBearerTokenNoClaims_ReturnsEnvironmentUserName()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DefaultUserProvider>>();
            var provider = new BearerTokenUserProvider(logger);
            var expectedUserName = Environment.UserName;

            var jwtToken = GenerateToken(Array.Empty<Claim>());

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer " + jwtToken;

            // Act
            var userName = provider.GetUserName(httpContext);

            // Assert
            Assert.Equal(expectedUserName, userName);
        }

        [Fact]
        public void GetUserName_WithValidBearerToken_ReturnsNameFromToken()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DefaultUserProvider>>();
            var provider = new BearerTokenUserProvider(logger);
            var expectedUserName = "JohnDoe";

            var claims = new List<Claim>
            {
                new Claim("unique_name", expectedUserName),
                new Claim("ipaddr", "192.168.1.1")
            };
            var jwtToken = GenerateToken(claims);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer " + jwtToken;

            // Act
            var userName = provider.GetUserName(httpContext);

            // Assert
            Assert.Equal(expectedUserName, userName);
        }

        [Fact]
        public void GetUserName_WithEmailClaimBearerToken_ReturnsEmailFromToken()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DefaultUserProvider>>();
            var provider = new BearerTokenUserProvider(logger);
            var expectedEmail = "johndoe@gmail.com";

            var claims = new List<Claim>
            {
                new Claim("email", expectedEmail),
            };
            var jwtToken = GenerateToken(claims);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer " + jwtToken;

            // Act
            var userName = provider.GetUserName(httpContext);

            // Assert
            Assert.Equal(expectedEmail, userName);
        }

        private string GenerateToken(IEnumerable<Claim> claims)
        {
            var keyBytes = Encoding.UTF8.GetBytes(Secret);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var now = DateTime.UtcNow;
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = now.Add(TimeSpan.FromMinutes(1)),
                SigningCredentials = credentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtSecurityTokenHandler.WriteToken(token);
            return jwtToken;
        }

        private const string Secret = "bYIJTgsdEGjoAWOkNAOez9UgTQicR9lP";
    }
}