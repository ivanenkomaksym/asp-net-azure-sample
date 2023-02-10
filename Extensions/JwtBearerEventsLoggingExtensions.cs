namespace AspNetAzureSample.Extensions
{
    internal static class JwtBearerEventsLoggingExtensions
    {
        public static void AuthenticationFailed(this ILogger logger, Exception e)
        {
            logger.LogError("Authentication failed Exception: {0}", e);
        }
        public static void TokenReceived(this ILogger logger)
        {
            logger.LogInformation("Received a bearer token");
        }
        public static void TokenValidationStarted(this ILogger logger, string userId, string issuer)
        {
            logger.LogInformation("Token Validation Started for User: {0} Issuer: {1}", userId, issuer);
        }
        public static void TokenValidationFailed(this ILogger logger, string userId, string issuer)
        {
            logger.LogWarning("Tenant is not registered User: {0} Issuer: {1}", userId, issuer);
        }
        public static void TokenValidationSucceeded(this ILogger logger, string userId, string issuer, string scope, string role)
        {
            logger.LogInformation("Token validation succeeded: User: {0} Issuer: {1} Scope: {2} Role: {3}", userId, issuer, scope, role);
        }
    }
}
