using System;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Infrastructure.Eventing.Messages;
using MassTransit.AzureServiceBus.Services.RequestResponse.CQRS;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Eventing.MassTransit.RequestResponse.Consumers.MediatRConsumer", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Infrastructure.Eventing
{
    public class MediatRConsumer<TMessage> : IConsumer<TMessage>
        where TMessage : class
    {
        private readonly IServiceProvider _serviceProvider;

        public MediatRConsumer(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task Consume(ConsumeContext<TMessage> context)
        {
            var eventBus = _serviceProvider.GetRequiredService<MassTransitEventBus>();
            eventBus.ConsumeContext = context;

            var sender = _serviceProvider.GetRequiredService<ISender>();
            object request;
            if (context.Message is IMapperRequest mapperRequest)
            {
                request = mapperRequest.CreateRequest();
            }
            else
            {
                request = context.Message;
            }
            var response = await sender.Send(request, context.CancellationToken);

            switch (response)
            {
                case null:
                case MediatR.Unit:
                    await context.RespondAsync(RequestCompletedMessage.Instance);
                    break;
                case not MediatR.Unit:
                    var mappedResponse = ResponseMappingFactory.CreateResponseMessage(response);
                    await context.RespondAsync(mappedResponse);
                    break;
            }
        }
    }

    public class MediatRConsumerDefinition<TMessage> : ConsumerDefinition<MediatRConsumer<TMessage>>
        where TMessage : class
    {
        protected override void ConfigureConsumer(
            IReceiveEndpointConfigurator endpointConfigurator,
            IConsumerConfigurator<MediatRConsumer<TMessage>> consumerConfigurator,
            IRegistrationContext context)
        {
        }
    }
}