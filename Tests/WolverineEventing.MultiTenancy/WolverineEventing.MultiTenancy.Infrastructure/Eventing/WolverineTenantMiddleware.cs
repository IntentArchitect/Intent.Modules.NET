using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using Wolverine;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineTenantMiddleware", Version = "1.0")]

namespace WolverineEventing.MultiTenancy.Infrastructure.Eventing
{
    public static class WolverineTenantMiddleware
    {
        public static async Task BeforeAsync(
            Envelope envelope,
            ITenantResolver tenantResolver,
            IMultiTenantContextSetter contextSetter)
        {
            if (string.IsNullOrEmpty(envelope.TenantId)) return;

            contextSetter.MultiTenantContext = await tenantResolver.ResolveAsync(envelope);
        }
    }
}