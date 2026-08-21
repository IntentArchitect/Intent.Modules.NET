using Intent.RoslynWeaver.Attributes;
using MediatR;
using Wolverine.Publish.RabbitMQ.Application.Common.Eventing;
using Wolverine.Publish.RabbitMQ.Eventing.Messages;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace Wolverine.Publish.RabbitMQ.Application.RequestOrderProcessing
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class RequestOrderProcessingCommandHandler : IRequestHandler<RequestOrderProcessingCommand>
    {
        private readonly IMessageBus _messageBus;

        [IntentManaged(Mode.Merge)]
        public RequestOrderProcessingCommandHandler(IMessageBus messageBus)
        {
            _messageBus = messageBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Fully)]
        public async Task Handle(RequestOrderProcessingCommand request, CancellationToken cancellationToken)
        {
            _messageBus.Send(new ProcessOrderCommand
            {
            });
        }
    }
}