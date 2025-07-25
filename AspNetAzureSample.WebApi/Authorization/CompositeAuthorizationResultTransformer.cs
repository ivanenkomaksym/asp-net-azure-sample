using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace AspNetAzureSample.Authorization
{
    public class CompositeAuthorizationResultTransformer : IAuthorizationMiddlewareResultHandler
    {
        private IEnumerable<IAuthorizationMiddlewareResultHandler> _handlers;

        private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new AuthorizationMiddlewareResultHandler();

        public CompositeAuthorizationResultTransformer(IEnumerable<IAuthorizationMiddlewareResultHandler> handlers)
        {
            _handlers = handlers ?? throw new ArgumentNullException(nameof(handlers));
        }
        public async Task HandleAsync(RequestDelegate next,
                                      HttpContext context,
                                      AuthorizationPolicy policy,
                                      PolicyAuthorizationResult authorizeResult)
        {
            foreach (var handler in _handlers)
            {
                await handler.HandleAsync(next, context, policy, authorizeResult);

                if (context.Response.HasStarted)
                {
                    // If the response has already started, we can stop processing further handlers
                    return;
                }
            }

            await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
        }
    }
}
