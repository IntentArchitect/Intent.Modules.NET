using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Configuration;
using Wolverine;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.Wolverine.WolverineTenantHeaderStrategy", Version = "1.0")]

namespace WolverineEventing.MultiTenancy.Infrastructure.Eventing
{
    public class WolverineTenantHeaderStrategy
    {
        public const string HeaderNameConfigurationKey = "Wolverine:TenantHeader";
        public const string DefaultHeaderName = "Tenant-Identifier";
        private readonly string _headerName;
        private readonly IMultiTenantContextAccessor _multiTenantContextAccessor;

        public WolverineTenantHeaderStrategy(IMultiTenantContextAccessor multiTenantContextAccessor,
            IConfiguration configuration)
        {
            _multiTenantContextAccessor = multiTenantContextAccessor;
            _headerName = ResolveHeaderName(configuration);
        }

        public static string ResolveHeaderName(IConfiguration configuration)
        {
            return configuration.GetValue<string?>(HeaderNameConfigurationKey) ?? DefaultHeaderName;
        }

        public DeliveryOptions? BuildDeliveryOptions()
        {
            var tenantIdentifier = _multiTenantContextAccessor.MultiTenantContext?.TenantInfo?.Identifier;

            if (tenantIdentifier is null)
            {
                return null;
            }

            var options = new DeliveryOptions();
            options.Headers[_headerName] = tenantIdentifier;
            return options;
        }
    }
}