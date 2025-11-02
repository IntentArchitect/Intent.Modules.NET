using System;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using AzureFunction.QueueStorage.Application.Common.Eventing;
using AzureFunction.QueueStorage.Domain.Common.Interfaces;
using AzureFunction.QueueStorage.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureQueueStorage.AzureFunctionConsumer", Version = "1.0")]

namespace AzureFunction.QueueStorage.Api
{
    public class CreateProductCommandConsumer
    {
        private readonly JsonSerializerOptions _serializerOptions;
        private readonly IAzureQueueStorageEventDispatcher _dispatcher;
        private readonly ILogger<CreateProductCommandConsumer> _logger;
        private readonly IEventBus _eventBus;
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;

        public CreateProductCommandConsumer(IAzureQueueStorageEventDispatcher dispatcher,
            ILogger<CreateProductCommandConsumer> logger,
            IEventBus eventBus,
            IServiceProvider serviceProvider,
            IUnitOfWork unitOfWork)
        {
            _dispatcher = dispatcher;
            _logger = logger;
            _eventBus = eventBus;
            _serviceProvider = serviceProvider;
            _serializerOptions = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            };
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("CreateProductCommandConsumer")]
        public async Task Run(
            [QueueTrigger("cleanarchitecture-queuestorage-eventing-messages-createproductcommand", Connection = "QueueStorage:DefaultEndpoint")] AzureQueueStorageEnvelope message,
            CancellationToken cancellationToken)
        {
            try
            {
                // The execution is wrapped in a transaction scope to ensure that if any other
                // SaveChanges calls to the data source (e.g. EF Core) are called, that they are
                // transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-
                // locks are released, while write-locks are maintained for the duration of the
                // transaction). Learn more on this approach for EF Core:
                // https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _dispatcher.DispatchAsync(_serviceProvider, message, _serializerOptions, cancellationToken);

                    // By calling SaveChanges at the last point in the transaction ensures that write-
                    // locks in the database are created and then released as quickly as possible. This
                    // helps optimize the application to handle a higher degree of concurrency.
                    await _unitOfWork.SaveChangesAsync(cancellationToken);

                    // Commit transaction if everything succeeds, transaction will auto-rollback when
                    // disposed if anything failed.
                    transaction.Complete();
                }
                await _eventBus.FlushAllAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing CreateProductCommandConsumer");
                throw;
            }
        }
    }
}