using System;
using System.Collections.Generic;
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
        private readonly Queue<Func<Task>> _actions = new();

        public StateRepository(DaprClient daprClient)
        {
            _daprClient = daprClient;
        }

        public void Update<T>(string id, T state)
        {
            _actions.Enqueue(async () =>
            {
                var currentState = await _daprClient.GetStateEntryAsync<T>(StateStoreName, id);
                currentState.Value = state;
                await currentState.SaveAsync();
            });
        }

        public async Task<T> Get<T>(string id, CancellationToken cancellationToken = default)
        {
            return await _daprClient.GetStateAsync<T>(StateStoreName, id, cancellationToken: cancellationToken);
        }

        public void Delete(string id)
        {
            _actions.Enqueue(async () =>
            {
                await _daprClient.DeleteStateAsync(StateStoreName, id);
            });
        }

        public async Task FlushAllAsync(CancellationToken cancellationToken = default)
        {
            while (_actions.TryDequeue(out var action))
            {
                await action();
            }
        }
    }
}