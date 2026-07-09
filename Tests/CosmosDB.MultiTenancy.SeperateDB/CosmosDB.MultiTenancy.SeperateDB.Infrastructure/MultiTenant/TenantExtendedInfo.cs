using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.MultiTenant
{
    public class TenantExtendedInfo : TenantInfo
    {
        public string? ConnectionString { get; set; }
    }
}