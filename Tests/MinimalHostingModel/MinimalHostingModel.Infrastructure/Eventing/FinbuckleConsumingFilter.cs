using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.FinbuckleConsumingFilter", Version = "1.0")]

namespace MinimalHostingModel.Infrastructure.Eventing
{
    public class FinbuckleConsumingFilter<T> : IFilter<ConsumeContext<T>>
        where T : class
    {
        private readonly string _headerName;
        private readonly FinbuckleMessageHeaderStrategy _messageHeaderStrategy;
        private readonly IMultiTenantContextAccessor _accessor;
        private readonly ITenantResolver _tenantResolver;

        public FinbuckleConsumingFilter(IServiceProvider serviceProvider,
            IMultiTenantContextAccessor accessor,
            ITenantResolver tenantResolver,
            IConfiguration configuration)
        {
            _accessor = accessor;
            _tenantResolver = tenantResolver;
            _headerName = configuration.GetValue<string?>("MassTransit:TenantHeader") ?? "Tenant-Identifier";

            _messageHeaderStrategy = (FinbuckleMessageHeaderStrategy)serviceProvider
                .GetRequiredService<IEnumerable<IMultiTenantStrategy>>()
                .Single(s => s.GetType() == typeof(FinbuckleMessageHeaderStrategy));
        }

        public async Task Send(ConsumeContext<T> context, IPipe<ConsumeContext<T>> next)
        {
            if (context.TryGetHeader<string>(_headerName, out var tenantIdentifier))
            {
                _messageHeaderStrategy.SetTenantIdentifier(tenantIdentifier);
                var multiTenantContext = await _tenantResolver.ResolveAsync(context);
                _accessor.MultiTenantContext = multiTenantContext;
            }
            await next.Send(context);
        }

        public void Probe(ProbeContext context)
        {
        }
    }
}