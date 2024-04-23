using System;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.CosmosRepository.Options;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBMultiTenantOptionsMonitor", Version = "1.0")]

namespace CosmosDB.MultiTenancy.SeperateDB.Infrastructure.Persistence
{
    public class CosmosDBMultiTenantOptionsMonitor : IOptionsMonitor<RepositoryOptions>
    {
        private readonly RepositoryOptions _options;

        public CosmosDBMultiTenantOptionsMonitor(RepositoryOptions options)
        {
            _options = options;
        }

        public RepositoryOptions CurrentValue => _options;

        public RepositoryOptions Get(string? name)
        {
            return _options;
        }

        public IDisposable? OnChange(Action<RepositoryOptions, string?> listener)
        {
            return null;
        }
    }
}