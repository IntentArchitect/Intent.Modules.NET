using System.Transactions;
using AspNetCore.AzureServiceBus.GroupB.Application.Common.Eventing;
using AspNetCore.AzureServiceBus.GroupB.Domain.Common.Interfaces;
using AspNetCore.AzureServiceBus.GroupB.Infrastructure.Configuration;
using AspNetCore.AzureServiceBus.GroupB.Infrastructure.Eventing;
using Azure.Messaging.ServiceBus;
using Microsoft.Extensions.Options;

namespace AspNetCore.AzureServiceBus.GroupB.Api.Services;

public class AzureServiceBusHostedService : BackgroundService, IAsyncDisposable
{
    private readonly IAzureServiceBusMessageDispatcher _dispatcher;
    private readonly ServiceBusClient _serviceBusClient;
    private readonly IServiceProvider _rootServiceProvider;
    private readonly ILogger<AzureServiceBusHostedService> _logger;
    private readonly SubscriptionOptions _subscriptionOptions;
    private readonly List<ServiceBusProcessor> _processors = [];

    public AzureServiceBusHostedService(
        IServiceProvider rootServiceProvider,
        IAzureServiceBusMessageDispatcher dispatcher,
        ServiceBusClient serviceBusClient,
        ILogger<AzureServiceBusHostedService> logger,
        IOptions<SubscriptionOptions> subscriptionOptions)
    {
        _dispatcher = dispatcher;
        _serviceBusClient = serviceBusClient;
        _rootServiceProvider = rootServiceProvider;
        _logger = logger;
        _subscriptionOptions = subscriptionOptions.Value;
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

        // Keep running until cancellation
        await Task.Delay(Timeout.Infinite, stoppingToken);
    }

    private ServiceBusProcessor CreateProcessor(SubscriptionEntry subscription, ServiceBusClient serviceBusClient)
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
            
            var unitOfWork = scopedServiceProvider.GetRequiredService<IUnitOfWork>();
            var eventBus = scopedServiceProvider.GetRequiredService<IEventBus>();
            
            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                       new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted },
                       TransactionScopeAsyncFlowOption.Enabled))
            {
                await _dispatcher.DispatchAsync(scopedServiceProvider, args.Message, cancellationToken);
                await unitOfWork.SaveChangesAsync(cancellationToken);
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

    public async ValueTask DisposeAsync()
    {
        foreach (var processor in _processors)
        {
            await processor.StopProcessingAsync();
            await processor.DisposeAsync();
        }
    }
}