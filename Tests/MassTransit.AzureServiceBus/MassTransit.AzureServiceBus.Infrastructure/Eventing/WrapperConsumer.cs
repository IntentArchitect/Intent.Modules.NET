using System;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.WrapperConsumer", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Infrastructure.Eventing
{
    public class WrapperConsumer<THandler, TMessage> : IConsumer<TMessage>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;

        public WrapperConsumer(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var eventBus = _serviceProvider.GetService<MassTransitEventBus>()!;
            eventBus.Current = context;

            using (var transaction = new TransactionScope(TransactionScopeOption.Required,
                new TransactionOptions() { IsolationLevel = IsolationLevel.ReadCommitted }, TransactionScopeAsyncFlowOption.Enabled))
            {
                var handler = _serviceProvider.GetService<THandler>()!;
                await handler.HandleAsync(context.Message, context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
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
        }
    }
}