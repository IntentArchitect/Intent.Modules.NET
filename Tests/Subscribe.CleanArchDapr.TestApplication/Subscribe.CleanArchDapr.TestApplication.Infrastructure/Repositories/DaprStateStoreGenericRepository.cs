using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;
using Subscribe.CleanArchDapr.TestApplication.Domain.Common.Interfaces;
using Subscribe.CleanArchDapr.TestApplication.Domain.Repositories;
using Subscribe.CleanArchDapr.TestApplication.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreGenericRepository", Version = "1.0")]

namespace Subscribe.CleanArchDapr.TestApplication.Infrastructure.Repositories
{
    public class DaprStateStoreGenericRepository : IDaprStateStoreGenericRepository
    {
        private const string StateStoreName = "statestore";
        private readonly DaprClient _daprClient;
        private readonly DaprStateStoreUnitOfWork _unitOfWork;
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _actions = new();

        public DaprStateStoreGenericRepository(
            DaprClient daprClient,
            DaprStateStoreUnitOfWork unitOfWork)
        {
            _daprClient = daprClient;
            _unitOfWork = unitOfWork;
        }

        public void Upsert<TValue>(string key, TValue state)
        {
            _unitOfWork.Enqueue(async cancellationToken =>
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
            _unitOfWork.Enqueue(async cancellationToken =>
            {
                await _daprClient.DeleteStateAsync(StateStoreName, key, cancellationToken: cancellationToken);
            });
        }

        public IDaprStateStoreUnitOfWork UnitOfWork => _unitOfWork;
    }
}