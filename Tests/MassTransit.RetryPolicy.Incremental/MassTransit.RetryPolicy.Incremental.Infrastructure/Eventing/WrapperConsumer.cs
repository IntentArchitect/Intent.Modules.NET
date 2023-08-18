using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.RetryPolicy.Incremental.Application.Common.Eventing;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.WrapperConsumer", Version = "1.0")]

namespace MassTransit.RetryPolicy.Incremental.Infrastructure.Eventing
{
    public class WrapperConsumer<THandler, TMessage> : IConsumer<TMessage>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;

        public WrapperConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var eventBus = _serviceProvider.GetService<MassTransitEventBus>()!;
            eventBus.Current = context;
            var handler = _serviceProvider.GetService<THandler>()!;
            await handler.HandleAsync(context.Message, context.CancellationToken);
            await eventBus.FlushAllAsync(context.CancellationToken);
        }
    }

    public class WrapperConsumerDefinition<THandler, TMessage> : ConsumerDefinition<WrapperConsumer<THandler, TMessage>>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;

        public WrapperConsumerDefinition(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<WrapperConsumer<THandler, TMessage>> consumerConfigurator)
        {
            endpointConfigurator.UseInMemoryInboxOutbox(_serviceProvider);
        }
    }
}