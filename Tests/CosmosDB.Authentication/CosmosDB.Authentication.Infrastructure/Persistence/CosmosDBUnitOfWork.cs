using System.Collections.Concurrent;
using CosmosDB.Authentication.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.CosmosDB.CosmosDBUnitOfWork", Version = "1.0")]

namespace CosmosDB.Authentication.Infrastructure.Persistence
{
    internal class CosmosDBUnitOfWork : ICosmosDBUnitOfWork
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _actions = new();
        private readonly ConcurrentDictionary<object, byte> _trackedEntities = new();

        public void Track(object? entity)
        {
            if (entity is null)
            {
                return;
            }
            _trackedEntities.TryAdd(entity, default);
        }

        public void Enqueue(Func<CancellationToken, Task> action)
        {
            _actions.Enqueue(action);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            while (_actions.TryDequeue(out var action))
            {
                await action(cancellationToken);
            }
        }
    }
}