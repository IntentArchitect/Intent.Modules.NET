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
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<AzureServiceBusHostedService> _logger;
    private readonly SubscriptionOptions _subscriptionOptions;
    private readonly List<ServiceBusProcessor> _processors = [];

    public AzureServiceBusHostedService(
        IAzureServiceBusMessageDispatcher dispatcher,
        IServiceProvider serviceProvider,
        ILogger<AzureServiceBusHostedService> logger,
        IOptions<SubscriptionOptions> subscriptionOptions)
    {
        _dispatcher = dispatcher;
        _serviceProvider = serviceProvider;
        _logger = logger;
        _subscriptionOptions = subscriptionOptions.Value;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var configuration =_serviceProvider.GetRequiredService<IConfiguration>();
        await using var serviceBusClient = new ServiceBusClient(configuration["AzureServiceBus:ConnectionString"]);
        foreach (var subscription in _subscriptionOptions.Entries)
        {
            var processor = CreateProcessor(subscription, serviceBusClient);
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
        return subscription.SubscriptionName != null
            ? serviceBusClient.CreateProcessor(subscription.QueueOrTopicName, subscription.SubscriptionName)
            : serviceBusClient.CreateProcessor(subscription.QueueOrTopicName);
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args, CancellationToken cancellationToken)
    {
        try
        {
            // Create scope for this specific message processing
            using var scope = _serviceProvider.CreateScope();
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
            throw; // Let ServiceBus handle retry/dead letter
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