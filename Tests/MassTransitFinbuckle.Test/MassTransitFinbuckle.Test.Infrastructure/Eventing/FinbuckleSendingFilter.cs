using System.Threading.Tasks;
using Finbuckle.MultiTenant;
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
        private readonly ITenantInfo _tenant;

        public FinbuckleSendingFilter(ITenantInfo tenant, IConfiguration configuration)
        {
            _tenant = tenant;
            _headerName = configuration.GetValue<string?>("MassTransit:TenantHeader") ?? "Tenant-Identifier";
        }

        public void Probe(ProbeContext context)
        {
        }

        public Task Send(SendContext<T> context, IPipe<SendContext<T>> next)
        {
            context.Headers.Set(_headerName, _tenant.Identifier);
            return next.Send(context);
        }
    }
}