using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.TenantConnectionsInterfaceTemplate", Version = "1.0")]

namespace Google.Cloud.Storage.Multitenancy.SeperateAccount.Tests.Infrastructure.MultiTenant
{
    public interface ITenantConnections
    {
        string? Id { get; set; }
        string? GoogleCloudStorageConnection { get; set; }
    }
}