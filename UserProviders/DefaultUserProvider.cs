namespace AspNetAzureSample.UserProviders
{
    public class DefaultUserProvider : IUserProvider
    {
        public DefaultUserProvider(ILogger<DefaultUserProvider> logger)
        {
            Logger = logger;
        }

        public virtual string GetUserName(HttpContext httpContext)
        {
            var user = httpContext.User;

            if (user.Identity.IsAuthenticated)
            {
                Logger.DefaultUserProviderUserNameReceived(user.Identity.Name);
                return user.Identity.Name;
            }

            Logger.DefaultUserProviderEnvironmentUserNameReceived(Environment.UserName);

            return Environment.UserName;
        }

        protected readonly ILogger<DefaultUserProvider> Logger;
    }
}
