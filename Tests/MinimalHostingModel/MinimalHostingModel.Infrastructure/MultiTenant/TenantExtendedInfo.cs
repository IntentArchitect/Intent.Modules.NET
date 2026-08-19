using Finbuckle.MultiTenant;
using Finbuckle.MultiTenant.Abstractions;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo", Version = "1.0")]

namespace MinimalHostingModel.Infrastructure.MultiTenant
{
    public class TenantExtendedInfo : TenantInfo
    {
        public string? ConnectionString { get; set; }
    }
}