using System.Transactions;
using Azure.Messaging.ServiceBus;
using AzureIdentityManagement.Application.Common.Eventing;
using AzureIdentityManagement.Domain.Common.Interfaces;
using AzureIdentityManagement.Infrastructure.Configuration;
using AzureIdentityManagement.Infrastructure.Eventing;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.Options;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.AzureServiceBus.AzureServiceBusHostedService", Version = "1.0")]

namespace AzureIdentityManagement.Api.Services
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
            AzureServiceBusSubscriptionEntry subscription,
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
                var eventBus = scopedServiceProvider.GetRequiredService<IMessageBus>();
                var cosmosDBUnitOfWork = scopedServiceProvider.GetRequiredService<ICosmosDBUnitOfWork>();
                var tableStorageUnitOfWork = scopedServiceProvider.GetRequiredService<ITableStorageUnitOfWork>();
                await _dispatcher.DispatchAsync(scopedServiceProvider, args.Message, cancellationToken);
                await tableStorageUnitOfWork.SaveChangesAsync(cancellationToken);
                await cosmosDBUnitOfWork.SaveChangesAsync(cancellationToken);
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