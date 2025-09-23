using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AwsLambdaFunction.Domain.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Aws.DynamoDB.DynamoDBUnitOfWork", Version = "1.0")]

namespace AwsLambdaFunction.Infrastructure.Persistence
{
    internal class DynamoDBUnitOfWork : IDynamoDBUnitOfWork
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _actions = new();
        // ConcurrentDictionary used due to .NET not having a "ConcurrentSet", values are not used
        private readonly ConcurrentDictionary<object, object?> _trackedEntities = new();

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
            while (_actions.TryDequeue(out var action))
            {
                await action(cancellationToken);
            }
        }
    }
}