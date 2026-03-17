using Intent.RoslynWeaver.Attributes;
using MassTransit.RequestResponse.Client.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MassTransit.RequestResponse.Client.Application.TestCommands.AzureServiceBusTest
{
    public class AzureServiceBusTestCommand : IRequest, ICommand
    {
        public AzureServiceBusTestCommand(string message)
        {
            Message = message;
        }

        public string Message { get; set; }
    }
}