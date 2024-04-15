namespace AspNetAzureSample.UserProviders
{
    public interface IUserProvider
    {
        public abstract string GetUserName(HttpContext httpContext);
    }
}
