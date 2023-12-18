using System.Security.Claims;

namespace AspNetAzureSample.Authentication
{
    /// <summary>
    /// This middleware will make sure to return authenticated user without real authentication. It is intended to be used in testing environment.
    /// Client can pass name and id in request headers.
    /// </summary>
    internal class AuthenticatedTestRequestMiddleware
    {
        public const string TestingAuthenticationScheme = "TestingAuthentication";
        public const string TestingNameHeader = "X-Testing-Name";
        public const string TestingNameIdHeader = "X-Testing-NameId";

        private readonly RequestDelegate _next;

        public AuthenticatedTestRequestMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Headers.Keys.Contains(TestingNameHeader))
            {
                var name = context.Request.Headers[TestingNameHeader].First();
                var id = context.Request.Headers.Keys.Contains(TestingNameIdHeader) ? context.Request.Headers[TestingNameIdHeader].First() : "";

                var claimsIdentity = new ClaimsIdentity(new List<Claim>
                {
                    new Claim(ClaimTypes.Name, name),
                    new Claim(ClaimTypes.NameIdentifier, id),
                }, TestingAuthenticationScheme);

                var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);
                context.User = claimsPrincipal;
            }

            await _next(context);
        }
    }
}
