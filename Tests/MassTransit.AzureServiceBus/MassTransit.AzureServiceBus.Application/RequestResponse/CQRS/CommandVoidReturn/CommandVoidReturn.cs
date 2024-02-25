using Intent.RoslynWeaver.Attributes;
using MassTransit.AzureServiceBus.Application.Common.Interfaces;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace MassTransit.AzureServiceBus.Application.RequestResponse.CQRS.CommandVoidReturn
{
    public class CommandVoidReturn : IRequest, ICommand
    {
        public CommandVoidReturn(string input)
        {
            Input = input;
        }

        public string Input { get; set; }
    }
}