using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using CleanArchitecture.Dapr.Domain.Common.Interfaces;
using CleanArchitecture.Dapr.Infrastructure.Persistence;
using Dapr.Client;
using Intent.RoslynWeaver.Attributes;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Dapr.AspNetCore.StateManagement.DaprStateStoreRepositoryBase", Version = "1.0")]

namespace CleanArchitecture.Dapr.Infrastructure.Repositories
{
    public abstract class DaprStateStoreRepositoryBase<TDomain>
    {
        private readonly ConcurrentBag<StateTransactionRequest> _transactionRequests = new();
        private readonly object _lock = new();
        private readonly DaprClient _daprClient;
        private readonly DaprStateStoreUnitOfWork _unitOfWork;
        private readonly bool _enableTransactions;
        private readonly string _storeName;

        protected DaprStateStoreRepositoryBase(DaprClient daprClient,
            DaprStateStoreUnitOfWork unitOfWork,
            bool enableTransactions,
            string storeName)
        {
            _daprClient = daprClient;
            _unitOfWork = unitOfWork;
            _enableTransactions = enableTransactions;
            _storeName = storeName;
        }

        public IDaprStateStoreUnitOfWork UnitOfWork => _unitOfWork;

        public async Task<TDomain> FindByKeyAsync(string key, CancellationToken cancellationToken = default)
        {
            var entity = await _daprClient.GetStateAsync<TDomain>(
                storeName: _storeName,
                key: key,
                cancellationToken: cancellationToken,
                metadata: new Dictionary<string, string>
                {
                    ["contentType"] = "application/json"
                });

            _unitOfWork.Track(entity);

            return entity;
        }

        protected async Task<List<TDomain>> FindByKeysAsync(string[] ids, CancellationToken cancellationToken = default)
        {
            var result = await _daprClient.GetBulkStateAsync(
                storeName: _storeName,
                keys: ids,
                parallelism: 1,
                cancellationToken: cancellationToken,
                metadata: new Dictionary<string, string>
                {
                    ["contentType"] = "application/json"
                });

            var entities = result
                .Select(x => JsonSerializer.Deserialize<TDomain>(x.Value, new JsonSerializerOptions { PropertyNameCaseInsensitive = true })!)
                .ToList();

            foreach (var entity in entities)
            {
                _unitOfWork.Track(entity);
            }

            return entities;
        }

        protected void Upsert(string key, TDomain entity)
        {
            _unitOfWork.Track(entity);

            if (_enableTransactions)
            {
                EnqueueTransactionRequest(new StateTransactionRequest(
                    key: key,
                    value: JsonSerializer.SerializeToUtf8Bytes(entity),
                    operationType: StateOperationType.Upsert,
                    metadata: new Dictionary<string, string>
                    {
                        ["contentType"] = "application/json",
                    }));
                return;
            }

            _unitOfWork.Enqueue(async cancellationToken =>
            {
                await _daprClient.SaveStateAsync(
                    storeName: _storeName,
                    key: key,
                    value: entity,
                    cancellationToken: cancellationToken,
                    metadata: new Dictionary<string, string>
                    {
                        ["contentType"] = "application/json"
                    });
            });
        }

        protected void Remove(string key, TDomain entity)
        {
            _unitOfWork.Track(entity);

            if (_enableTransactions)
            {
                EnqueueTransactionRequest(new StateTransactionRequest(
                    key: key,
                    value: null,
                    operationType: StateOperationType.Delete));
                return;
            }

            _unitOfWork.Enqueue(async cancellationToken =>
            {
                await _daprClient.DeleteStateAsync(
                    storeName: _storeName,
                    key: key,
                    cancellationToken: cancellationToken);
            });
        }

        public async Task<List<TDomain>> FindAllAsync(CancellationToken cancellationToken = default)
        {
            var result = await _daprClient.QueryStateAsync<TDomain>(
                storeName: _storeName,
                jsonQuery: "{ \"filter: \": {}}",
                cancellationToken: cancellationToken,
                metadata: new Dictionary<string, string>
                {
                    ["contentType"] = "application/json",
                    ["queryIndexName"] = "key"
                });

            var entities = result.Results
                .Select(x => x.Data)
                .ToList();

            foreach (var entity in entities)
            {
                _unitOfWork.Track(entity);
            }

            return entities;
        }

        private void EnqueueTransactionRequest(StateTransactionRequest request)
        {
            if (_transactionRequests.IsEmpty)
            {
                lock (_lock)
                {
                    if (_transactionRequests.IsEmpty)
                    {
                        _unitOfWork.Enqueue(async cancellationToken =>
                        {
                            await _daprClient.ExecuteStateTransactionAsync(
                                storeName: _storeName,
                                operations: _transactionRequests.ToArray(),
                                cancellationToken: cancellationToken);
                        });
                    }
                }
            }

            _transactionRequests.Add(request);
        }
    }
}