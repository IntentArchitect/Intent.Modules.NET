using System.Threading.Tasks;
using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.FinbuckleSendingFilter", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Infrastructure.Eventing
{
    public class FinbuckleSendingFilter<T> : IFilter<SendContext<T>>
        where T : class
    {
        private readonly string _headerName;
        private readonly IMultiTenantContextAccessor _multiTenantContextAccessor;

        public FinbuckleSendingFilter(IMultiTenantContextAccessor multiTenantContextAccessor, IConfiguration configuration)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _headerName = configuration.GetValue<string?>("MassTransit:TenantHeader") ?? "Tenant-Identifier";
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            var tenantIdentifier = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier;
            if (tenantIdentifier is not null)
            {
                context.Headers.Set(_headerName, tenantIdentifier);
            }
            return next.Send(context);
        }
    }
}
