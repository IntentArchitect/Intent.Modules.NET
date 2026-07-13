using System.Threading.Tasks;
using Finbuckle.MultiTenant;
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
            if (context.RequestId.HasValue
    && context.TryGetPayload<ConsumeContext>(out var sourceConsumeContext)
    && sourceConsumeContext.RequestId == context.RequestId)
            {
                // This is a MassTransit-generated reply/fault correlating to a previously consumed
                // request (RespondAsync, or an unhandled-exception Fault) - it is correlation-routed
                // via RequestId, not tenant-routed, so it can proceed without a resolved tenant. This
                // legitimately happens when UseInMemoryOutbox/UseInMemoryInboxOutbox defers the send
                // until after the consumer's AsyncLocal-based Finbuckle tenant context has unwound.
                var replyTenantIdentifier = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier;
                if (replyTenantIdentifier is not null)
                {
                    context.Headers.Set(_headerName, replyTenantIdentifier);
                }

                return next.Send(context);
            }
            var tenantIdentifier = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier
                    ?? throw new MultiTenantException("Cannot send a message without a resolved tenant context.");
            context.Headers.Set(_headerName, tenantIdentifier);
            return next.Send(context);
        }
    }
}
