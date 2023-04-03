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

        public static void BearerTokenUserProviderUserNameReceived(this ILogger logger, string userName)
        {
            logger.LogInformation($"BearerTokenUserProvider: received '{userName}' user name.");
        }
    }
}
