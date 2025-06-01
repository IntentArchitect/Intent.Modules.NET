using AzureFunctions.AzureServiceBus.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.AzureServiceBus.Application.Client.CreateClient
{
    public class CreateClientCommand : IRequest, ICommand
    {
        public CreateClientCommand(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }
}