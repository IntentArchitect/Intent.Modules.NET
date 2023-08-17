using System;
using System.Threading.Tasks;
using System.Transactions;
using Intent.RoslynWeaver.Attributes;
using MassTransit;
using MassTransitFinbuckle.Test.Application.Common.Eventing;
using MassTransitFinbuckle.Test.Domain.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.WrapperConsumer", Version = "1.0")]

namespace MassTransitFinbuckle.Test.Infrastructure.Eventing
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
                await eventBus.FlushAllAsync(context.CancellationToken);
                await _unitOfWork.SaveChangesAsync(context.CancellationToken);
                transaction.Complete();
            }
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