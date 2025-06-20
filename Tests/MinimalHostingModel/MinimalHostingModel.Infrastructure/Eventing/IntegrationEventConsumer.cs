using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using MinimalHostingModel.Application.Common.Eventing;
using MinimalHostingModel.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventConsumer", Version = "1.0")]

namespace MinimalHostingModel.Infrastructure.Eventing
{
    public class IntegrationEventConsumer<THandler, TMessage> : IConsumer<TMessage>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IDistributedCacheWithUnitOfWork _distributedCacheWithUnitOfWork;

        public IntegrationEventConsumer(IServiceProvider serviceProvider,
            IDistributedCacheWithUnitOfWork distributedCacheWithUnitOfWork)
        {
            _serviceProvider = serviceProvider;
            _distributedCacheWithUnitOfWork = distributedCacheWithUnitOfWork ?? throw new ArgumentNullException(nameof(distributedCacheWithUnitOfWork));
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var eventBus = _serviceProvider.GetRequiredService<MassTransitEventBus>();
            eventBus.ConsumeContext = context;
            var handler = _serviceProvider.GetRequiredService<THandler>();

            using (_distributedCacheWithUnitOfWork.EnableUnitOfWork())
            {
                await handler.HandleAsync(context.Message, context.CancellationToken);
                await _distributedCacheWithUnitOfWork.SaveChangesAsync(context.CancellationToken);
            }
            await eventBus.FlushAllAsync(context.CancellationToken);
        }
    }

    public class IntegrationEventConsumerDefinition<THandler, TMessage> : ConsumerDefinition<IntegrationEventConsumer<THandler, TMessage>>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<IntegrationEventConsumer<THandler, TMessage>> consumerConfigurator,
            IRegistrationContext context)
        {
            endpointConfigurator.UseInMemoryInboxOutbox(context);
        }
    }
}