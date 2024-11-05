using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Modules.AspNetCore.MultiTenancy.TenantConnectionsInterfaceTemplate", Version = "1.0")]

namespace MongoDb.MultiTenancy.SeperateDb.Infrastructure.MultiTenant
{
    public interface ITenantConnections
    {
        string? Id { get; set; }
        string? MongoDbConnection { get; set; }
    }
}