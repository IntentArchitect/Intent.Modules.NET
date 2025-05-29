using AzureFunctions.AzureEventGrid.Application.Common.Interfaces;
using Intent.RoslynWeaver.Attributes;
using MediatR;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace AzureFunctions.AzureEventGrid.Application.CreateClient
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