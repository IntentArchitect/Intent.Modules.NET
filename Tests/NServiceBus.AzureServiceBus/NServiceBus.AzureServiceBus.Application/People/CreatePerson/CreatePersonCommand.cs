using Intent.RoslynWeaver.Attributes;
using MediatR;
using NServiceBus.AzureServiceBus.Application.Common.Interfaces;

[assembly: DefaultIntentManaged(Mode.Fully)]
[assembly: IntentTemplate("Intent.Application.MediatR.CommandModels", Version = "1.0")]

namespace NServiceBus.AzureServiceBus.Application.People.CreatePerson
{
    public class CreatePersonCommand : IRequest, ICommand
    {
        public CreatePersonCommand()
        {
        }
    }
}