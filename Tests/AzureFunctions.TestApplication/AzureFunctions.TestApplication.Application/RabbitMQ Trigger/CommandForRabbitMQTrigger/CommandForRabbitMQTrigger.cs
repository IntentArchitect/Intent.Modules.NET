using AzureFunctions.TestApplication.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.TestApplication.Application.RabbitMQTrigger.CommandForRabbitMQTrigger
{
    public class CommandForRabbitMQTrigger : IRequest, ICommand
    {
        public CommandForRabbitMQTrigger()
        {
        }
    }
}