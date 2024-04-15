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
        public void GetUserName_WithValidBearerToken_ReturnsNameFromToken()
        {
            // Arrange
            var logger = Substitute.For<ILogger<DefaultUserProvider>>();
            var provider = new BearerTokenUserProvider(logger);

            var keyBytes = Encoding.UTF8.GetBytes(Secret);
            var securityKey = new SymmetricSecurityKey(keyBytes);
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
            var now = DateTime.UtcNow;
            var claims = new List<Claim>
            {
                new Claim("unique_name", "JohnDoe"),
                new Claim("ipaddr", "192.168.1.1")
            };
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = now.Add(TimeSpan.FromMinutes(1)),
                SigningCredentials = credentials
            };

            var jwtSecurityTokenHandler = new JwtSecurityTokenHandler();
            var token = jwtSecurityTokenHandler.CreateToken(tokenDescriptor);
            var jwtToken = jwtSecurityTokenHandler.WriteToken(token);

            var httpContext = new DefaultHttpContext();
            httpContext.Request.Headers["Authorization"] = "Bearer " + jwtToken;

            // Act
            var userName = provider.GetUserName(httpContext);

            // Assert
            Assert.Equal("JohnDoe", userName);
        }

        private const string Secret = "4777E6A4592BF1C9D5E62BFB41A77";
    }
}