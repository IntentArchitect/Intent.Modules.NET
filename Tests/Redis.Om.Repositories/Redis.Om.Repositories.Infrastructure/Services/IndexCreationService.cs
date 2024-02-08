using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Hosting;
using Redis.OM;
using Redis.Om.Repositories.Infrastructure.Persistence.Documents;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Redis.Om.Repositories.Templates.IndexCreationService", Version = "1.0")]

namespace Redis.Om.Repositories.Infrastructure.Services
{
    public class IndexCreationService : IHostedService
    {
        private readonly RedisConnectionProvider _provider;

        public IndexCreationService(RedisConnectionProvider provider)
        {
            _provider = provider;
        }

        public async Task StartAsync(CancellationToken cancellationToken = default)
        {
            await _provider.Connection.CreateIndexAsync(typeof(BaseTypeDocument));
            await _provider.Connection.CreateIndexAsync(typeof(ClientDocument));
            await _provider.Connection.CreateIndexAsync(typeof(CustomerDocument));
            await _provider.Connection.CreateIndexAsync(typeof(DerivedOfTDocument));
            await _provider.Connection.CreateIndexAsync(typeof(DerivedTypeDocument));
            await _provider.Connection.CreateIndexAsync(typeof(IdTestingDocument));
            await _provider.Connection.CreateIndexAsync(typeof(InvoiceDocument));
            await _provider.Connection.CreateIndexAsync(typeof(RegionDocument));
        }

        public Task StopAsync(CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}