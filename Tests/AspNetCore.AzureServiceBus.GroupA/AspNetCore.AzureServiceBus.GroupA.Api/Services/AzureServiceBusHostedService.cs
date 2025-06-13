using System.Transactions;
using AspNetCore.AzureServiceBus.GroupA.Application.Common.Eventing;
using AspNetCore.AzureServiceBus.GroupA.Domain.Common.Interfaces;
using AspNetCore.AzureServiceBus.GroupA.Infrastructure.Configuration;
using AspNetCore.AzureServiceBus.GroupA.Infrastructure.Eventing;
using Azure.Messaging.ServiceBus;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusHostedService", Version = "1.0")]

namespace AspNetCore.AzureServiceBus.GroupA.Api.Services
{
    public class AzureServiceBusHostedService : BackgroundService, IAsyncDisposable
    {
        private readonly IServiceProvider _rootServiceProvider;
        private readonly IAzureServiceBusMessageDispatcher _dispatcher;
        private readonly ServiceBusClient _serviceBusClient;
        private readonly ILogger<AzureServiceBusHostedService> _logger;
        private readonly AzureServiceBusSubscriptionOptions _subscriptionOptions;
        private readonly List<ServiceBusProcessor> _processors = [];

        public AzureServiceBusHostedService(IServiceProvider rootServiceProvider,
            IAzureServiceBusMessageDispatcher dispatcher,
            ServiceBusClient serviceBusClient,
            ILogger<AzureServiceBusHostedService> logger,
            IOptions<AzureServiceBusSubscriptionOptions> subscriptionOptions)
        {
            _rootServiceProvider = rootServiceProvider;
            _dispatcher = dispatcher;
            _serviceBusClient = serviceBusClient;
            _logger = logger;
            _subscriptionOptions = subscriptionOptions.Value;
        }

        public async ValueTask DisposeAsync()
        {
            foreach (var processor in _processors)
            {
                await processor.StopProcessingAsync();
                await processor.DisposeAsync();
            }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            foreach (var subscription in _subscriptionOptions.Entries)
            {
                var processor = CreateProcessor(subscription, _serviceBusClient);
                processor.ProcessMessageAsync += args => ProcessMessageAsync(args, stoppingToken);
                processor.ProcessErrorAsync += ProcessErrorAsync;
                _processors.Add(processor);
                await processor.StartProcessingAsync(stoppingToken);
            }
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }

        private static ServiceBusProcessor CreateProcessor(
            SubscriptionEntry subscription,
            ServiceBusClient serviceBusClient)
        {
            var options = new ServiceBusProcessorOptions
            {
                AutoCompleteMessages = false,
                MaxConcurrentCalls = 1,
                PrefetchCount = 0
            };
            return subscription.SubscriptionName != null
                ? serviceBusClient.CreateProcessor(subscription.QueueOrTopicName, subscription.SubscriptionName, options)
                : serviceBusClient.CreateProcessor(subscription.QueueOrTopicName, options);
        }

        private async Task ProcessMessageAsync(ProcessMessageEventArgs args, CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _rootServiceProvider.CreateScope();
                var scopedServiceProvider = scope.ServiceProvider;
                var eventBus = scopedServiceProvider.GetRequiredService<IEventBus>();
                var unitOfWork = scopedServiceProvider.GetRequiredService<IUnitOfWork>();

                // The execution is wrapped in a transaction scope to ensure that if any other
                // SaveChanges calls to the data source (e.g. EF Core) are called, that they are
                // transacted atomically. The isolation is set to ReadCommitted by default (i.e. read-
                // locks are released, while write-locks are maintained for the duration of the
                // transaction). Learn more on this approach for EF Core:
                // https://docs.microsoft.com/en-us/ef/core/saving/transactions#using-systemtransactions
                using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                    new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    await _dispatcher.DispatchAsync(scopedServiceProvider, args.Message, cancellationToken);

                    // By calling SaveChanges at the last point in the transaction ensures that write-
                    // locks in the database are created and then released as quickly as possible. This
                    // helps optimize the application to handle a higher degree of concurrency.
                    await unitOfWork.SaveChangesAsync(cancellationToken);

                    // Commit transaction if everything succeeds, transaction will auto-rollback when
                    // disposed if anything failed.
                    transaction.Complete();
                }
                await eventBus.FlushAllAsync(cancellationToken);
                await args.CompleteMessageAsync(args.Message, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing ServiceBus message");
                throw;
            }
        }

        private Task ProcessErrorAsync(ProcessErrorEventArgs args)
        {
            _logger.LogError(args.Exception, "ServiceBus processing error");
            return Task.CompletedTask;
        }
    }
}