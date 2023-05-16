using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;
using Publish.CleanArchDapr.TestApplication.Domain.Repositories;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.StateRepository", Version = "1.0")]

namespace Publish.CleanArchDapr.TestApplication.Infrastructure.Repositories
{
    public class StateRepository : IStateRepository
    {
        private const string StateStoreName = "statestore";
        private readonly DaprClient _daprClient;
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _actions = new();

        public StateRepository(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public void Upsert<TValue>(string key, TValue state)
        {
            _actions.Enqueue(async cancellationToken =>
            {
                var currentState = await _daprClient.GetStateEntryAsync<TValue>(StateStoreName, key, cancellationToken: cancellationToken);
                currentState.Value = state;
                await currentState.SaveAsync(cancellationToken: cancellationToken);
            });
        }

        public async Task<TValue> GetAsync<TValue>(string key, CancellationToken cancellationToken = default)
        {
            return await _daprClient.GetStateAsync<TValue>(StateStoreName, key, cancellationToken: cancellationToken);
        }

        public void Delete(string key)
        {
            _actions.Enqueue(async cancellationToken =>
            {
                await _daprClient.DeleteStateAsync(StateStoreName, key, cancellationToken: cancellationToken);
            });
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            while (_actions.TryDequeue(out var action))
            {
                await action(cancellationToken);
            }
        }
    }
}