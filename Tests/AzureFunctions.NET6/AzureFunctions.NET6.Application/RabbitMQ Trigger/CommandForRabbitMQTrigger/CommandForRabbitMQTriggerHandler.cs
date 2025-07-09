using System;
using System.Threading;
using System.Threading.Tasks;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandHandler", Version = "2.0")]

namespace AzureFunctions.NET6.Application.RabbitMQTrigger.CommandForRabbitMQTrigger
{
    [IntentManaged(Mode.Merge, Signature = Mode.Fully)]
    public class CommandForRabbitMQTriggerHandler : IRequestHandler<CommandForRabbitMQTrigger>
    {
        [IntentManaged(Mode.Merge)]
        public CommandForRabbitMQTriggerHandler()
        {
        }

        [IntentManaged(Mode.Fully, Body = Mode.Merge)]
        public async Task Handle(CommandForRabbitMQTrigger request, CancellationToken cancellationToken)
        {
            // TODO: Implement Handle (CommandForRabbitMQTriggerHandler) functionality
            throw new NotImplementedException("Your implementation here...");
        }
    }
}