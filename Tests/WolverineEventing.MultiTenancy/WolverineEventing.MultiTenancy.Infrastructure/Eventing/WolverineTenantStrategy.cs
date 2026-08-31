using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using Wolverine;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineTenantStrategy", Version = "1.0")]

namespace WolverineEventing.MultiTenancy.Infrastructure.Eventing
{
    public class WolverineTenantStrategy : IMultiTenantStrategy
    {
        public Task<string?> GetIdentifierAsync(object context)
        {
            return Task.FromResult((context as Envelope)?.TenantId);
        }
    }
}