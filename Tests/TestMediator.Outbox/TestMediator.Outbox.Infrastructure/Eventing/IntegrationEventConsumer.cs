using Intent.RoslynWeaver.Attributes;
using MassTransit;
using Microsoft.Extensions.DependencyInjection;
using TestMediator.Outbox.Application.Common.Eventing;
using TestMediator.Outbox.Domain.Common.Interfaces;
using TestMediator.Outbox.Infrastructure.Persistence;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.IntegrationEventConsumer", Version = "1.0")]

namespace TestMediator.Outbox.Infrastructure.Eventing
{
    public class IntegrationEventConsumer<THandler, TMessage> : IConsumer<TMessage>
        where TMessage : class
        where THandler : IIntegrationEventHandler<TMessage>
    {
        private readonly IServiceProvider _serviceProvider;
        private readonly IUnitOfWork _unitOfWork;

        public IntegrationEventConsumer(IServiceProvider serviceProvider, IUnitOfWork unitOfWork)
        {
            _serviceProvider = serviceProvider;
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var messageBus = _serviceProvider.GetRequiredService<MassTransitMessageBus>();
            messageBus.ConsumeContext = context;
            var handler = _serviceProvider.GetRequiredService<THandler>();
            await handler.HandleAsync(context.Message, context.CancellationToken);
            await _unitOfWork.SaveChangesAsync(context.CancellationToken);
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
            endpointConfigurator.UseEntityFrameworkOutbox<ApplicationDbContext>(context);
        }
    }
}