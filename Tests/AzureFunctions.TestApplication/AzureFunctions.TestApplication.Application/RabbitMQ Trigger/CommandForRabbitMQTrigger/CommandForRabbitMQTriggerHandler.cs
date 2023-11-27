using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.TestApplication.Application.RabbitMQTrigger.CommandForRabbitMQTrigger
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandForRabbitMQTriggerHandler : IRequestHandler<CommandForRabbitMQTrigger>
    {
        [IntentManaged(Mode.Merge)]
        public CommandForRabbitMQTriggerHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Ignore)]
        public async Task Handle(CommandForRabbitMQTrigger request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException("Your implementation here...");
        }
    }
}