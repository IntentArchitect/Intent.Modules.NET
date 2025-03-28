using Finbuckle.MultiTenant;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.TenantExtendedInfo", Version = "1.0")]

namespace Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.MultiTenant
{
    public class TenantExtendedInfo : TenantInfo, ITenantConnections
    {
        public string? GoogleCloudStorageConnection { get; set; }
    }
}