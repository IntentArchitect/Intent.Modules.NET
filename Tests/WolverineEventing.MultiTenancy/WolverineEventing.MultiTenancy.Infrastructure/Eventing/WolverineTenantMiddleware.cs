using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineTenantMiddleware", Version = "1.0")]

namespace WolverineEventing.MultiTenancy.Infrastructure.Eventing
{
    public static class WolverineTenantMiddleware
    {
        public static IMultiTenantContext? Before(
            Envelope envelope,
            ITenantResolver tenantResolver,
            IMultiTenantContextAccessor contextAccessor,
            IMultiTenantContextSetter contextSetter,
            IConfiguration configuration)
        {
            var previous = contextAccessor.MultiTenantContext;

            var headerName = WolverineTenantHeaderStrategy.ResolveHeaderName(configuration);

            if (!envelope.Headers.TryGetValue(headerName, out var tenantId) || string.IsNullOrEmpty(tenantId))
            {
                return previous;
            }

            contextSetter.MultiTenantContext = tenantResolver.ResolveAsync(envelope).GetAwaiter().GetResult();
            return previous;
        }

        public static Task FinallyAsync(IMultiTenantContext? previous, IMultiTenantContextSetter contextSetter)
        {
            contextSetter.MultiTenantContext = previous!;

            return Task.CompletedTask;
        }
    }
}