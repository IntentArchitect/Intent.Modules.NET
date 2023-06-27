using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Eventing;
using MassTransit.AzureServiceBus.Eventing.Messages;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.Test.SendTest
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SendTestCommandHandler : IRequestHandler<SendTestCommand>
    {
        private readonly IEventBus _eventBus;

        [IntentManaged(Mode.Merge)]
        public SendTestCommandHandler(IEventBus eventBus)
        {
            _eventBus = eventBus;
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task<Unit> Handle(SendTestCommand request, CancellationToken cancellationToken)
        {
            _eventBus.Publish(new TestMessageEvent() { Message = request.Message });
            return Unit.Value;
        }
    }
}