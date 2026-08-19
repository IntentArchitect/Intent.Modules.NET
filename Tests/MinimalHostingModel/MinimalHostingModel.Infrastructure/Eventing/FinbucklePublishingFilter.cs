using System.Threading.Tasks;
using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.Configuration;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.FinbucklePublishingFilter", Version = "1.0")]

namespace MinimalHostingModel.Infrastructure.Eventing
{
    public class FinbucklePublishingFilter<T> : IFilter<PublishContext<T>>
        where T : class
    {
        private readonly string _headerName;
        private readonly IMultiTenantContextAccessor _multiTenantContextAccessor;

        public FinbucklePublishingFilter(IMultiTenantContextAccessor multiTenantContextAccessor, IConfiguration configuration)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _headerName = configuration.GetValue<string?>("MassTransit:TenantHeader") ?? "Tenant-Identifier";
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
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