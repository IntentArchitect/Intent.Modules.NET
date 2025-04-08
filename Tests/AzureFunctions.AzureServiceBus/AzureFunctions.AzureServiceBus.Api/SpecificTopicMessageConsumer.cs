using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Azure.Messaging.ServiceBus;
using AzureFunctions.AzureServiceBus.Application.Common.Eventing;
using AzureFunctions.AzureServiceBus.Domain.Common.Interfaces;
using AzureFunctions.AzureServiceBus.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.AzureFunctions.AzureServiceBus.AzureFunctionConsumer", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Api
{
    public class SpecificTopicMessageConsumer
    {
        private readonly IAzureServiceBusMessageDispatcher _dispatcher;
        private readonly ILogger<SpecificTopicMessageConsumer> _logger;
        private readonly IEventBus _eventBus;
        private readonly IUnitOfWork _unitOfWork;

        public SpecificTopicMessageConsumer(IAzureServiceBusMessageDispatcher dispatcher,
            ILogger<SpecificTopicMessageConsumer> logger,
            IEventBus eventBus,
            IUnitOfWork unitOfWork)
        {
            _dispatcher = dispatcher;
            _logger = logger;
            _eventBus = eventBus;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        [Function("SpecificTopicMessageConsumer")]
        public async Task Run(
            [ServiceBusTrigger("%AzureServiceBus:SpecificTopic%", "%AzureServiceBus:SpecificTopicSubscription%", Connection = "AzureServiceBus:ConnectionString")] ServiceBusReceivedMessage message,
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
                    await _dispatcher.DispatchAsync(message, cancellationToken);

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
                _logger.LogError(ex, "Error processing SpecificTopicMessageConsumer");
                throw;
            }
        }
    }
}