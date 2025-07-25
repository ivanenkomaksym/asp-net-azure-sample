using Microsoft.AspNetCore.Authorization;

namespace AspNetAzureSample.Authorization
{
    public static class AuthorizationResultTransformerExtensions
    {
        public static void AddCompositeAuthorizationResultTransformer(this IServiceCollection services,
                                                                      params Type[] handlerTypes)
        {
            foreach (var handlerType in handlerTypes)
            {
                if (!typeof(IAuthorizationMiddlewareResultHandler).IsAssignableFrom(handlerType))
                    throw new ArgumentException($"Type {handlerType.FullName} does not implement '{nameof(IAuthorizationMiddlewareResultHandler)}'.");

                services.AddSingleton(handlerType);
            }

            services.AddSingleton<IAuthorizationMiddlewareResultHandler>(serviceProvider =>
            {
                var handlers = new List<IAuthorizationMiddlewareResultHandler>();

                foreach (var handlerType in handlerTypes)
                {
                    var handler = serviceProvider.GetRequiredService(handlerType) as IAuthorizationMiddlewareResultHandler;
                    handlers.Add(handler);
                }
                return new CompositeAuthorizationResultTransformer(handlers);
            });
        }
    }
}
