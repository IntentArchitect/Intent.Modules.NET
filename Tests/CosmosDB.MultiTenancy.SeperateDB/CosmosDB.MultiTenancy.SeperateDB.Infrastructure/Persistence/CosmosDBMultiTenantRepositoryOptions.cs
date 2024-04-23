using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Azure.CosmosRepository.Providers;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBMultiTenantRepositoryOptions", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence
{
    public class CosmosDBMultiTenantRepositoryOptions : RepositoryOptions
    {
        private readonly CosmosDBMultiTenantClientProvider _clientProvider;

        public CosmosDBMultiTenantRepositoryOptions(ICosmosClientProvider clientProvider)
        {
            _clientProvider = (CosmosDBMultiTenantClientProvider)clientProvider;
        }

        public override string ContainerId
        {
            get => _clientProvider.GetDefaultContainer();
            set { }
        }
        public override string DatabaseId
        {
            get => _clientProvider.GetDatabase();
            set { }
        }
        public override string? CosmosConnectionString
        {
            get => _clientProvider.Tenant?.ConnectionString;
            set { }
        }
    }
}