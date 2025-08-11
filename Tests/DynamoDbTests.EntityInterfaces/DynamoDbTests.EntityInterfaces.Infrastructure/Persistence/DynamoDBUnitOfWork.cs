using System.Collections.Concurrent;
using DynamoDbTests.EntityInterfaces.Application.Common.Interfaces;
using DynamoDbTests.EntityInterfaces.Domain.Common;
using DynamoDbTests.EntityInterfaces.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBUnitOfWork", Version = "1.0")]

namespace DynamoDbTests.EntityInterfaces.Infrastructure.Persistence
{
    internal class DynamoDBUnitOfWork : IDynamoDBUnitOfWork
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _actions = new();
        // ConcurrentDictionary used due to .NET not having a "ConcurrentSet", values are not used
        private readonly ConcurrentDictionary<object, object?> _trackedEntities = new();
        private readonly IDomainEventService _domainEventService;

        public DynamoDBUnitOfWork(IDomainEventService domainEventService)
        {
            _domainEventService = domainEventService;
        }

        public void Track(object? entity)
        {
            if (entity is null)
            {
                return;
            }

            _trackedEntities.TryAdd(entity, null);
        }

        public void Enqueue(Func<CancellationToken, Task> action)
        {
            _actions.Enqueue(action);
        }

        public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            await DispatchEvents(cancellationToken);
            while (_actions.TryDequeue(out var action))
            {
                await action(cancellationToken);
            }
        }

        private async Task DispatchEvents(CancellationToken cancellationToken = default)
        {
            while (true)
            {
                var domainEventEntity = _trackedEntities
                    .Keys
                    .OfType<IHasDomainEvent>()
                    .SelectMany(x => x.DomainEvents)
                    .FirstOrDefault(domainEvent => !domainEvent.IsPublished);

                if (domainEventEntity is null)
                {
                    break;
                }

                domainEventEntity.IsPublished = true;
                await _domainEventService.Publish(domainEventEntity, cancellationToken);
            }
        }
    }
}