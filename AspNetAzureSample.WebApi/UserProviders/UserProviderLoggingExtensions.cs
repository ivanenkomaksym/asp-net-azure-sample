using System.Security.Claims;

namespace AspNetAzureSample.UserProviders
{
    internal static class UserProviderLoggingExtensions
    {
        public static void DefaultUserProviderUserNameReceived(this ILogger logger, string userName)
        {
            logger.LogInformation($"DefaultUserProvider: received '{userName}' user name.");
        }

        public static void DefaultUserProviderEnvironmentUserNameReceived(this ILogger logger, string userName)
        {
            logger.LogInformation($"DefaultUserProvider: received default environment '{userName}' user name.");
        }

        public static void BearerTokenUserProviderUserNameReceived(this ILogger logger, string userName, Claim? ipaddrClaim)
        {
            logger.LogInformation($"BearerTokenUserProvider: received '{userName}' user name. IP address: {ipaddrClaim?.Value}");
        }

        public static void BearerTokenUserProviderEmailReceived(this ILogger logger, string email)
        {
            logger.LogInformation($"BearerTokenUserProvider: received '{email}' email.");
        }
    }
}
