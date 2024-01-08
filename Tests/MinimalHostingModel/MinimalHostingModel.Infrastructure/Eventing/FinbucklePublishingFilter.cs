using System.Threading.Tasks;
using Finbuckle.MultiTenant;
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
        private readonly ITenantInfo _tenant;

        public FinbucklePublishingFilter(ITenantInfo tenant, IConfiguration configuration)
        {
            _tenant = tenant;
            _headerName = configuration.GetValue<string?>("MassTransit:TenantHeader") ?? "Tenant-Identifier";
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task Send(PublishContext<T> context, IPipe<PublishContext<T>> next)
        {
            context.Headers.Set(_headerName, _tenant.Identifier);
            return next.Send(context);
        }
    }
}