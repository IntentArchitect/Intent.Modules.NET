using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace MassTransit.RabbitMQ.Application.NamingOverrides.SendFromEventHandler
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class SendFromEventHandlerCommandHandler : IRequestHandler<SendFromEventHandlerCommand>
    {
        [IntentManaged(Mode.Merge)]
        public SendFromEventHandlerCommandHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(SendFromEventHandlerCommand request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (SendFromEventHandlerCommandHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}