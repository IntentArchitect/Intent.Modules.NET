using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Transactions;
using Google.Cloud.PubSub.V1;
using Intent.RoslynWeaver.Attributes;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Publish.CleanArch.GooglePubSub.TestApplication.Application.Common.Eventing;
using Publish.CleanArch.GooglePubSub.TestApplication.Domain.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.GoogleCloud.PubSub.EventingTemplates.GoogleSubscriberBackgroundService", Version = "1.0")]

namespace Publish.CleanArch.GooglePubSub.TestApplication.Infrastructure.Eventing
{
    public class GoogleSubscriberBackgroundService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly string _subscriptionId;
        private readonly string _topicId;
        private SubscriberClient _subscriberClient;
        private readonly ILogger<GoogleSubscriberBackgroundService> _logger;

        public GoogleSubscriberBackgroundService(IServiceProvider serviceProvider, string subscriptionId, string topicId)
        {
            _serviceProvider = serviceProvider;
            _subscriptionId = subscriptionId;
            _topicId = topicId;
            _logger = serviceProvider.GetService<ILogger<GoogleSubscriberBackgroundService>>();
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            var resourceManager = _serviceProvider.GetService<ICloudResourceManager>();
            if (resourceManager.ShouldSetupCloudResources)
            {
                await resourceManager.CreateTopicIfNotExistAsync(_topicId, cancellationToken);
                await resourceManager.CreateSubscriptionIfNotExistAsync((_subscriptionId, _topicId), cancellationToken);
            }
            var subscriptionName = SubscriptionName.FromProjectSubscription(resourceManager.ProjectId, _subscriptionId);
            _subscriberClient = await SubscriberClient.CreateAsync(subscriptionName);
            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await _subscriberClient.StartAsync(RequestHandler);
            }
        }

        private async Task<SubscriberClient.Reply> RequestHandler(
            PubsubMessage message,
            CancellationToken cancellationToken)
        {
            try
            {
                using var scope = _serviceProvider.CreateScope();
                var eventBus = scope.ServiceProvider.GetService<IEventBus>();
                using (var transaction = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
                {
                    var unitOfWork = scope.ServiceProvider.GetService<IUnitOfWork>();
                    var subscriptionManager = scope.ServiceProvider.GetService<IEventBusSubscriptionManager>();
                    await subscriptionManager.DispatchAsync(scope.ServiceProvider, message, cancellationToken);
                    await unitOfWork.SaveChangesAsync(cancellationToken);
                    transaction.Complete();
                }
                await eventBus.FlushAllAsync(cancellationToken);
                return SubscriberClient.Reply.Ack;
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Error processing Pubsub Message.");
                throw;
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await _subscriberClient.StopAsync(cancellationToken);
            await base.StopAsync(cancellationToken);
        }
    }
}